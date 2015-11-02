using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace SS
{
   /// <summary>
   /// Keeps track of how many top-level forms are running
   /// </summary>
   class SSApplicationContext : ApplicationContext
   {
      // Number of open forms
      private int formCount = 0;

      // Singleton ApplicationContext
      private static SSApplicationContext appContext;

      /// <summary>
      /// Private constructor for singleton pattern
      /// </summary>
      private SSApplicationContext()
      {
      }

      /// <summary>
      /// Returns the one SSApplicationContext.
      /// </summary>
      /// <returns></returns>
      public static SSApplicationContext getAppContext()
      {
         if (appContext == null)
         {
            appContext = new SSApplicationContext();
         }
         return appContext;
      }

      /// <summary>
      /// Runs the form
      /// </summary>
      /// <param name="form"></param>
      public void RunForm(Form form)
      {
         // One more form is running
         formCount++;

         // When this form closes, we want to find out
         form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

         // Run the form
         form.Show();
      }

   }


   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         // Start an application context and run one form inside it
         SSApplicationContext appContext = SSApplicationContext.getAppContext();
         appContext.RunForm(new Form1());
         Application.Run(appContext);
      }
   }

}
