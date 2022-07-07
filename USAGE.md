Terminal applications are really easy to develop and even though you can build one using .NET command line tools it is a lot easier using Visual Studio or any other IDE of your choice.

All classes shown below belong to the <code>Lmpessoa.Terminal</code> namespace. You might want to import it globally.

## Creating the Application

The first step in making a Terminal application is to instantiate the <code>Application</code> class. That's as simple as this:

```cs
Application app = new();
```

That's it. No complicated parameters but also no interpreting console-based arguments. I usually do it on the <code>Program.cs</code> file because it allows for a cleaner code.

Note that being this simples does not mean that the application cannot be customised, but there are currently very few options if you need them. You can set up any of those options by supplying a lambda method for that to the constructor (just like you would do while configuring an ASP.NET application):

```cs
Application app = new(options => {
   // check the options var for what can be customised
});
```

## Mapping the screen contents

I've reused the very same name that **CSP** called screens when designing the class to represent one: map. Thus, in order to create a map, all you have to do is create a subclass inheriting from <code>Map</code> and you are almost done. When you create a new instance of your <code>Map</code> subclass, it will automatically try and load the screen definition from a file with the same name as the full name of the map class plus the extension <code>.map</code>. So, for example, if you have a class called <code>Coolreads.BookInfoMap</code>, the map will try to load an embedded resource called <code>Coolreads.BookInfoMap.map</code>. Remember the file must be built as an embedded resource into the application itself and while at it remember to account for the default namespace of your application.

In case you want to load your map from a different source, assembly or file, you can set it using the <code>Contents</code> (yes, you could even load it remotely if you wanted but I wouldn't recommend it):

```cs
public class BookInfoMap : Map
{
   public BookInfoMap()
   {
      Contents = LoadFileFromAnotherSource();
   }
}
```

The <code>Contents</code> property is write-only and can only be set on the map class constructor.

## What makes a map

Now that we have our <code>Application</code> in place and a class to make reference to our map, all we have to do is actually draw the screen.

For that, as I mentioned on the previous section, you can set the value of the <code>Contents</code> property or create an embedded file with the same full name of your map class with the extension <code>.map</code>. Here is an example from a library sample application:

```
>
>  coolreads                      ADD A NEW BOOK
> 
>       TITLE:¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>  SORT TITLE:¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>      AUTHOR:¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>        ISBN:¬9999999999999^          ASIN:¬A999999999^
>   PUBLISHER:¬XXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>   PUBLISHED YEAR:¬9999^  MONTH:¬99^
>       PAGES:¬9999^
>      FORMAT:¬X^PAPERBACK   ¬X^HARDCOVER   ¬X^E-BOOK
>     EDITION:¬99¬XXXXXXXXXXXXXXXXXXXXXXXXX^
> DESCRIPTION:¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>             ¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>             ¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>             ¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>             ¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>             ¬XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX^
>    LANGUAGE:¬XXXXXXXXXXXXXXXXXXXX^
> 
>  ____________________________________________________________________________
> ¬MS                                                                          ^
>  PF1 = HELP   PF3 = RETURN   PF10 = SAVE
```

As you can see, you get a pretty close view of how the screen will look like when rendered to the console. Note only the *greater than* symbol (<code>&gt;</code>) on the leftmost side of the line. It is there to indicates this is what will be shown when your map is loaded to the screen (sans the <code>&gt;</code>). Any lines with almost any other character at this position will be completely ignored but you will understand why we need this in a minute, just follow along for now.

Apart from the screen visible in plain text, the second thing you might have noticed in our map file is all those spaces filled with <code>X</code>s and some <code>9</code>s (there is an <code>A</code> there too; have you found it?). Those are placeholders for our fields and the space they consume is delimiter by a not symbol (<code>¬</code>, if you don't know how to type this, it usually is <code>**RIGHT_ALT+6**</code>) on the leftmost side of the field and the caret symbol (<code>&#94;</code>). You will notice those symbols themselves are not printed and are replaced by a regular whitespace. All fields must begin with a <code>¬</code> and end with a <code>&#94;</code> and must have at least one other character in between them, which will define what kind of data will be accepted for that field:
* <code>**X**</code> will accept any character the user types (a whitespace will have the same behaviour but I prefer using <code>X</code>s because I can see more clearly the length of the field).
* <code>**9**</code> will accept only decimal digits (values between 0 and 9).
* <code>**A**</code> will accept only Latin letters (values between A and Z, uppercase and lowercase).
* <code>**\***</code> will also accept any characters but their output will be masked, like a password.

There are also two other values that may appear between the field delimiters and they are only possible for fields with a length of at least 2 characters and may hold/display any value(those fields must only comprise of these characters followed by as many spaces required for its length and nothing else):

* <code>**RO**</code> means the field is read-only and not editable by the user.
* <code>**MS**</code> means the field is not only read-only but also a message status field. Each map may only contain one status field.

Also, if you look closely to our example, you will notice on the edition line of the form that there is an exception to the rule for defining. The regular rule for delimiting fields would mean we would need at least two blank spaces between two fields, to allow a minimum of one blank space the toolkit allow for the caret of the first field to be suppressed if followed by the start of the following field. 

## Showing the map

I know you will not believe me but that's it, you have a functional screen for your application. To show you that, all we must do is make the application we created previously show our map. To do that, all we have to do is call the <code>Run()</code> method on the application:

```cs
Application app = new();
app.Run(new BookInfoMap());
```

Go ahead, run it. I'll wait right here. Amazing isn't it.

Of course most applications are not made using a single screen and you might easily find yourself wanting to show more and more of them as the application grows and only the initial screen of your application is shown using the <code>Application</code> instance. But showing another map after the application is running is even easier: instantiate the map as before and call the method <code>Show()</code> in it.

```cs
BookInfoHelpMap help = new();
help.Show();
```

When a new map is shown, the previous map does not simply dies. Instead, the <code>Application</code> class automatically keeps track of each open map and allows you to navigate all the way back to the first map by just calling <code>Close()</code> on the current map (notice you can only close the map that is the current one being displayed).

Now you might be wondering why do I require you to instantiate the map class? I could have easily done it using reflection and dependency inversion but by having you instantiate it yourself makes the code simple and allows you to decide what is the best way to do it (design patterns involved or not). Also, that will enable you to pass data between screens with ease. 

Speaking of that...

## Getting data from fields

Have you noticed we did not declare any means to store or retrieve the values entered for each field? That's because it is also automatically handled for each map. The <code>Map</code> class defines a property called <code>Fields</code> that can reference any field defined in our map, the index of a field is the order in which it appears on the map itself. In our <code>BookInfoMap</code> example, the title field is index 0, sort title is index 1, and so forth.

To ease handling fields I strongly recommend defining a constant on your map class with the index of each field that is important to you.

```cs
   private const int TITLE = 0;
   private const int SORT_TITLE = 1;
```

The description field takes actually 6 fields (indices 14 to 19) and denotes a limitation of our toolkit (which would also be a limitation of mainframe applications built with CSP): if you need a field to contain more data than could fit in a single line, you will have to handle it using multiple fields.

With the same limitation spanning multiple fields is the format field. Using more common UI frameworks this field would be defined using an specific component that would allow for a single value to be chosen. For Terminal applications, it is up to the developer to decide how to handle the user selecting more than one option.

Another thing that must be taken in consideration regarding fields is that all values stored by the map are strings thus any conversion to a format that would best suit your application is up to you. For example, supposing there is an <code>enum</code> defined for the format field, it could be handled by the following property:

```cs
   public BookFormat Format {
      get => $"{Fields[10]}{Fields[11]}{Fields[12]}".ToUpper() switch {
         "X  " => BookFormat.Paperback,
         " X " => BookFormat.Hardcover,
         "  X" => BookFormat.Ebook,
         _ => throw new ArgumentException(),
      };
      set {
         Fields[10].Value = value is BookFormat.Paperback ? "X" : " ";
         Fields[11].Value = value is BookFormat.Hardcover ? "X" : " ";
         Fields[12].Value = value is BookFormat.Ebook ? "X" : " ";
      }
```

You may choose to handle other characters being used for those flag fields (I've already seen people using <code>/</code> but any character you accept will do).

## Making maps do things

So, any map you push to your application will give you instant screen design and edition and navigation between fields that you can use and cast to any format you might need. But do you remember that the last line of our map contained a set of keys associated with actions (*PF1* for help, *PF3* to go back/exit, and *PF6* to save)? Have you tried pushing those keys?

Okay, now you found that this toolkit is not so magical. There are many things you have to do by hand like declaring what pressing those (and maybe other) keys on the user's keyboard will do for each map. But don't worry, that is an easy task as well, and the last one to get any map ready to be used. Each map contains a method that can be overridden to provide key behaviour to them:

```cs
   override protected void OnKeyPressed(ConsoleKeyInfo key) {
      if(key.Modifiers != 0) {
         return;
      }
      switch (key.Key) {
         case ConsoleKey.F1:
            // Display another map with help
            BookInfoHelpMap help = new();
            help.ShowDialog();
            break;
         case ConsoleKey.F3:
            // Go back to the previous map
            Close();
            break;
         case ConsoleKey.F6:
            // Save the book data
            // ...
            break;
      }
   }
```

Here again the choice for how to handle keys pressed in code is up to you. It is even your choice to ignore modifiers or not.

## Finishing touches

Creating console applications using this Terminal is not hard, uh? There are just a few more things for you to take note for us to wrap up.

* If you look closely, you will see I didn't order the help map on the previous example to show up using the <code>Show()</code> method. The <code>ShowDialog()</code> method allows for smaller maps to be shown, not hiding the contents of the previous map completely. Only one map can be show as dialog per application and having a dialog map trying to show another map will raise an exception.

* Along with <code>OnKeyPressed()</code> there are other four events that can be handled in the life cycle of a map:

	* <code>OnActivating()</code> is called whenever a map is becoming the currently active map, whether it is being shown for the first time or by navigating back to it.

	* <code>OnDeactivating()</code> is called whenever a map is no longer the one being shown to the user. It happens both when a map is closed or before another is shown on top of it.

	* <code>OnClosing()</code> is the only one of the five events that requires a feedback. It must return a boolean value stating whether the current map can be closed or not. A value of false prevents the map from being closed.

	* <code>OnClosed()</code> happens when a map has been closed and allows you to close any resources it might still be using.
	
* Closing a map does not mean the map instance will be disposed, *au contraire* if the map that create the closed map call <code>Show()</code> on it again, the closed map will be shown again and will remember any values that were previously passed to or edited through it.

* You can drop all maps and immediately terminate the current application by calling <code>Application.Exit()</code>. It also allows you to pass a return code that can be returned by your application to the underlying shell, if needed. No events regarding closing and deactivating of maps on the navigation stack will be further processed.

* Terminal application can be deployed to remote users using SSH and even display a custom login map as part of your application. The trick to not having the user type two passwords (one for the SSH session and one for your application) is to configure an SSH connection authenticated by certificate and disable by password (some may disavow, but  using a single account with the least privileges for SSH and manage login and permissions through your app does the job -- don't get me started but that's exact what web applications do). 

* Out of curiosity, PF*n* keys are actually just regular F*n* keys when they were called *program function keys*. Feel free to spell them as you want.

## Guidelines for terminal UIs

If the modern developer has little to no contact at all with developing an actual console application, we might not be exaggerating to say their familiarity with the mainframe design is absolute zero. A few guidelines that may help a beginner map designer are:

* Use standardised headers for your maps throughout your application, it will also help you reduce your time designing them.

* Always put in your maps a (unique) title that helps users identify where they are/what are they doing.

* Always have a field to display error messages to the user on the bottom of the screen, usually it goes above the list of the function keys of the map

* Function keys and the description of their actions must follow on the bottom of the screen, usually below the error field.

* The Terminal toolkit will allow for many keys and combinations with modifier keys to be captured and handled but whenever you can prefer using F*n* keys for actions.

* It is exactly mainframe programming practice but you may use the method <code>MoveFocus()</code> to move focus to a field related to a validation error so the user immediately knows where the error message comes from (try using a different active field colour if you do that to draw the user's attention to the active field).

* If you are planning on developing a really big Terminal application, make sure you don't stack too many maps on your navigation stack, watch for the memory limits of the machine it will run later (and the number of concurrent users).

Also note that there is absolutely no demand that you follow these guidelines, you might just find out a new set of rules that works best for you, your applications and/or your users.