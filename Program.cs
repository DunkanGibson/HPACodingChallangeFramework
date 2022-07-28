using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace HPACodingChallangeFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Process[] pname = Process.GetProcessesByName("Notepad");
            if (pname.Length == 0)
            {
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "notepad.exe";
            myProcess.Start();
            Console.WriteLine("Now openining Notepad!");
            Thread.Sleep(1000);
            Console.WriteLine("Grabbing root window");
            AutomationElement rootElement = AutomationElement.RootElement;
            Thread.Sleep(1000);
            Console.WriteLine("Grabbing notepad window");
            AutomationElement notePadWindow = rootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ProcessIdProperty, myProcess.Id));
            Thread.Sleep(1000);
            Console.WriteLine("Getting the file menu item");
            AutomationElement file = notePadWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "File"));
            Thread.Sleep(1000);
            Console.WriteLine("Setting pattern for expansion of menu");
            ExpandCollapsePattern fileExpandPattern = file.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            Thread.Sleep(1000);
            Console.WriteLine("Expanding menu");
            fileExpandPattern.Expand();
            Thread.Sleep(1000);
            Console.WriteLine("Finding window and getting pattern");  
            AutomationElement newOption = file.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "New"));
            InvokePattern newClickPattern = newOption.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            Console.WriteLine("Selecting New option on menu");
            newClickPattern.Invoke();
            Thread.Sleep(1000);
            Console.WriteLine("Inserting text into notepad");
            AutomationElement textArea = notePadWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "15"));
            InsertTextUsingUIAutomation(textArea, "Hello World");
            Thread.Sleep(1000);
            Console.WriteLine("Expanding menu");
            fileExpandPattern.Expand();
            Thread.Sleep(1000);
            Console.WriteLine("Finding window and getting pattern");
            AutomationElement save = file.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Save As..."));
            InvokePattern savePattern = save.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            Console.WriteLine("Selecting Save option on menu");
            savePattern.Invoke();
            Thread.Sleep(1000);
            Console.WriteLine("Inserting text into Save dialog box");
            AutomationElement saveAsTextArea = notePadWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "1001"));
            InsertTextUsingUIAutomation(saveAsTextArea, "PathAndFileNameHelloWorld");
            Thread.Sleep(1000);
            Console.WriteLine("Clicking Save button");
            AutomationElement saveOption = rootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "1"));
            InvokePattern saveAsWindowDialogClickPattern = saveOption.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            saveAsWindowDialogClickPattern.Invoke();
            Console.WriteLine("Checking to see if there is a Confirm Save As box");
            AutomationElement isContentTextPresent = notePadWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "CommandButton_6"));
      
                if(isContentTextPresent != null)
                    {
                       Console.WriteLine("Finding window and getting pattern");
                       AutomationElement confirmSaveAsOption = notePadWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "CommandButton_6"));
                       InvokePattern confirmSaveAsWindowDialogClickPattern = confirmSaveAsOption.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                       Console.WriteLine("Clicking Confirm Save As button");
                       confirmSaveAsWindowDialogClickPattern.Invoke();
                    }              

            }
            else
            {
                Console.WriteLine("Notepad is already running. Please close all instances before running this app");
                Thread.Sleep(3000);
                return;
            }


            void InsertTextUsingUIAutomation(AutomationElement element, string value)
            {
                    // Validate arguments / initial setup
                    if (value == null)
                        throw new ArgumentNullException(
                            "String parameter must not be null.");

                    if (element == null)
                        throw new ArgumentNullException(
                            "AutomationElement parameter must not be null");

                    // A series of basic checks prior to attempting an insertion.
                    // Check #1: Is control enabled?
                    if (!element.Current.IsEnabled)
                    {
                        throw new InvalidOperationException(
                            "The control with an AutomationID of "
                            + element.Current.AutomationId.ToString()
                            + " is not enabled.\n\n");
                    }

                    // Check #2: Are there styles that prohibit us
                    //           from sending text to this control?
                    if (!element.Current.IsKeyboardFocusable)
                    {
                        throw new InvalidOperationException(
                            "The control with an AutomationID of "
                            + element.Current.AutomationId.ToString()
                            + "is read-only.\n\n");
                    }

                    // Once you have an instance of an AutomationElement,
                    // check if it supports the ValuePattern pattern.
                    object valuePattern = null;

                    if (!element.TryGetCurrentPattern(
                        ValuePattern.Pattern, out valuePattern))
                    {
                        // Set focus for input functionality and begin.
                        element.SetFocus();

                        // Pause before sending keyboard input.
                        Thread.Sleep(100);

                        // Delete existing content in the control and insert new content.
                        SendKeys.SendWait("^{HOME}");   // Move to start of control
                        SendKeys.SendWait("^+{END}");   // Select everything
                        SendKeys.SendWait("{DEL}");     // Delete selection
                        SendKeys.SendWait(value);
                    }
                    // Control supports the ValuePattern pattern so we can
                    // use the SetValue method to insert content.
                    else
                    {

                        // Set focus for input functionality and begin.
                        element.SetFocus();

                        ((ValuePattern)valuePattern).SetValue(value);
                    }
            }
        }
    }
}
