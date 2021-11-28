using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bibliotheque.forms
{
    /// <summary>
    /// Interaction logic for BorrowForm.xaml
    /// </summary>
    public partial class BorrowForm : Page
    {
        private PhpfribiblioContext context = Globals.databaseContext;
        private Book currentBook;
        private DateTime? startDate;
        private DateTime? endDate;
        private bool fromCode;

        private Borrow currentBorrow;

        public BorrowForm(Book book)
        {

            InitializeComponent();

            currentBook = book;

            tb_header.Text += $" {currentBook.Title}";

            calendar_booking.BlackoutDates.AddDatesInPast();

            List<Borrow> booking = context.Borrows.Where(c => c.BookId == currentBook.BookId && c.EndDate == null).ToList();
            foreach (Borrow borrow in booking)
            {
                calendar_booking.BlackoutDates.Add(new CalendarDateRange(borrow.StartDate, borrow.DueDate));
            }
        }
        /// <summary>
        /// Update Borrow dates
        /// </summary>
        /// <param name="borrow"></param>
        public BorrowForm(Borrow borrow)
        {

            InitializeComponent();

            currentBorrow = context.Borrows.Find(borrow.BorrowId);
            currentBook = currentBorrow.Book;

            tb_header.Text += $" {currentBook.Title}";

            calendar_booking.BlackoutDates.AddDatesInPast();

            List<Borrow> booking = context.Borrows.Where(c => c.BookId == currentBook.BookId && c.EndDate == null).ToList();

            foreach (Borrow bookborrow in booking)
            {
                if (borrow != bookborrow) //Exclude this borrow
                    calendar_booking.BlackoutDates.Add(new CalendarDateRange(bookborrow.StartDate, bookborrow.DueDate));
            }
            if (DateTime.Today.AddDays(-1) >= borrow.StartDate)
                calendar_booking.BlackoutDates[0].End = borrow.StartDate.AddDays(-1);

            calendar_booking.BlackoutDates.Remove(new CalendarDateRange(borrow.StartDate, borrow.DueDate));

            calendar_booking.SelectedDates.AddRange(borrow.StartDate, borrow.DueDate);
        }


        private void Click_AddBorrow(object sender, RoutedEventArgs e)
        {
            //Check if there's a start date, if no end date, the end dates is the same day as start day
            if (!AssertData()) return;

            //Add
            if (currentBorrow == null)
            {
                Borrow borrow = new Borrow();

                borrow.BookId = currentBook.BookId;
                borrow.AccountId = Globals.currentUser.AccountId;
                borrow.StartDate = (DateTime)startDate;
                borrow.DueDate = (DateTime)endDate;

                context.Add(borrow);
            }
            else //update
            {
                currentBorrow.StartDate = (DateTime)startDate;
                currentBorrow.DueDate = (DateTime)endDate;

                context.Update(currentBorrow);
            }

            context.SaveChanges();

            Globals.mainWindow.CloseForm();
        }

        private void Calendar_booking_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendar_booking.SelectedDate == null)
                return;

            //If selecting dates by dragging
            if (calendar_booking.SelectedDates.Count > 1)
            {
                startDate = calendar_booking.SelectedDates.First();
                endDate = calendar_booking.SelectedDates.Last();

                OrderDates(ref startDate, ref endDate);

                tb_startdate.Text = ((DateTime)startDate).ToShortDateString();
                tb_enddate.Text = ((DateTime)endDate).ToShortDateString();
                return;
            }


            if (fromCode) return;

            //Selected no date
            if (startDate == null)
            {
                startDate = calendar_booking.SelectedDate;
                tb_startdate.Text = ((DateTime)startDate).ToShortDateString();
            }
            else if (endDate == null) //Selected only the start date
            {
                endDate = (DateTime)calendar_booking.SelectedDate;
                //Reverse the positions if needed
                OrderDates(ref startDate, ref endDate);

                tb_enddate.Text = ((DateTime)endDate).ToShortDateString();

                //programatically highlight the date slots and check if they don't overlap with blackout days
                fromCode = true;
                for (DateTime date = (DateTime)startDate; date <= endDate; date = date.AddDays(1))
                    if (calendar_booking.BlackoutDates.Contains(date))
                    {
                        calendar_booking.SelectedDates.Clear();
                        tb_startdate.Text = null;
                        tb_enddate.Text = null;
                        fromCode = false;
                        return;
                    }

                calendar_booking.SelectedDates.AddRange((DateTime)startDate, (DateTime)endDate);
                fromCode = false;
            }
            else //Reset
            {

                fromCode = true;
                DateTime cache = (DateTime)calendar_booking.SelectedDate;
                calendar_booking.SelectedDates.Clear();
                calendar_booking.SelectedDates.Add(cache);

                startDate = cache;
                endDate = null;

                tb_startdate.Text = ((DateTime)startDate).ToShortDateString();
                tb_enddate.Text = null;
                fromCode = false;
            }
        }

        private bool AssertData()
        {
            bool isDataReady = true;

            tb_startdate_error.Visibility = Visibility.Collapsed;

            if (startDate == null)
            {
                tb_startdate_error.Visibility = Visibility.Visible;
                isDataReady = false;
            }
            if (endDate == null)
            {
                endDate = startDate;
            }

            return isDataReady;
        }

        private void Calendar_booking_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void OrderDates(ref DateTime? startDate, ref DateTime? endDate)
        {
            if (endDate < startDate)
            {
                DateTime cache = (DateTime)endDate;
                endDate = startDate;
                startDate = cache;
            }
        }
    }
}
