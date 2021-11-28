using Bibliotheque.helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Bibliotheque
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /*  protected override void OnStartup(StartupEventArgs e)
          {
              EventManager.RegisterClassHandler(typeof(TextBox), TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
              EventManager.RegisterClassHandler(typeof(CheckBox), CheckBox.KeyDownEvent, new KeyEventHandler(CheckBox_KeyDown));
              EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.KeyDownEvent, new KeyEventHandler(PasswordBox_KeyDown));

              base.OnStartup(e);
          }

          void TextBox_KeyDown(object sender, KeyEventArgs e)
          {
              if (e.Key == Key.Enter & (sender as TextBox).AcceptsReturn == false) MoveToNextUIElement(e);
              if (e.Key == Key.Down & (sender as TextBox).AcceptsReturn == false) MoveToNextUIElement(e);
              if (e.Key == Key.Up & (sender as TextBox).AcceptsReturn == false) MoveToPreviousUIElement(e);
          }

          void PasswordBox_KeyDown(object sender, KeyEventArgs e)
          {
              if (e.Key == Key.Enter) MoveToNextUIElement(e);
              if (e.Key == Key.Down) MoveToNextUIElement(e);
              if (e.Key == Key.Up) MoveToPreviousUIElement(e);
          }

          void CheckBox_KeyDown(object sender, KeyEventArgs e)
          {
              MoveToNextUIElement(e);
              //Sucessfully moved on and marked key as handled.
              //Toggle check box since the key was handled and
              //the checkbox will never receive it.
              if (e.Handled == true)
              {
                  CheckBox cb = (CheckBox)sender;
                  cb.IsChecked = !cb.IsChecked;
              }

          }

          void MoveToNextUIElement(KeyEventArgs e)
          {
              // Creating a FocusNavigationDirection object and setting it to a
              // local field that contains the direction selected.
              FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

              // MoveFocus takes a TraveralReqest as its argument.
              TraversalRequest request = new TraversalRequest(focusDirection);

              // Gets the element with keyboard focus.
              UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

              // Change keyboard focus.
              if (elementWithFocus != null)
              {
                  if (elementWithFocus.MoveFocus(request)) e.Handled = true;
              }
          }

          void MoveToPreviousUIElement(KeyEventArgs e)
          {
              // Creating a FocusNavigationDirection object and setting it to a
              // local field that contains the direction selected.
              FocusNavigationDirection focusDirection = FocusNavigationDirection.Previous;

              // MoveFocus takes a TraveralReqest as its argument.
              TraversalRequest request = new TraversalRequest(focusDirection);

              // Gets the element with keyboard focus.
              UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

              // Change keyboard focus.
              if (elementWithFocus != null)
              {
                  if (elementWithFocus.MoveFocus(request)) e.Handled = true;
              }
          }*/
    }
}
