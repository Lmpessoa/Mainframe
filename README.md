<img alt="Terminal icon" src="Resources/Header.svg" height="150">

Terminal is a simple visual user interface toolkit for developing console .NET applications.

## Why the Console?

Since the raise of the graphical user interface, the development of new console-based applications has been in decline, most often relegated to IT related work. GUI applications are visually more interesting, may provide richer data, be easier to use since you can just click wherever you want/need and be more appealing to users than applications based only on a set of characters laid on a (mostly) black and white (some times green) screen. So why would anyone bother to develop another console applications?

Anyone who has already developed a modern rich web/desktop/mobile application know the effort one must go through to get the UI right: control placements, in-place validations, screen navigation, choose colours, images, icons, and even when you don't have to develop the entire set of the visual experience of the application you still have to waste a lot of effort learning how to apply it correctly to get the desired visual results. It takes time, effort, most times a team composed of multiple people for front-end and back-end development due to using different technologies, that development and maintenance times can easily go through the roof.

Developing console-based applications weren't easier either, with the developer having to fine control where to "draw" the screen, where (and when) to capture user input, and so forth. But back in 2004 I got in contact with a development tool for mainframe applications called **CSP** that amazed me with the ability it provided to build an entire screen by literally writing how it would look like in plain text. The concept has been in the back of my head since then and I've even given lots of thought it could be interesting to develop applications for common platforms like that but never actually minded to do anything like it until now.

Thus came Terminal, a set of classes intended to help developers create rich console based applications that could be used either locally (for testing concepts, business logic, personal utilities and so forth) or remotely through an SSH connection even to a larger audience. Developing your application as a Terminal application is good because:

1. It is easier and faster to develop applications (especially for developers) when you can see the exact outcome screens of your software, graphical user interfaces are all about this. It takes almost no code at all to get a screen up and running.

2. Because it has fewer capabilities (in terms of visual) than other UI alternatives, you tend to waste less time and effort to build a functional application with a better looking UI than you can achieve using <code>Console.Write()</code> only (you may try out Miguel de Icasa's [gui.cs](https://github.com/migueldeicaza/gui.cs), it is more visually appealing but more verbose and you only get to see the resulting screen when you run your application).

3. Because code behind the UI is simpler/smaller (DLL is only 30KB), you need less code to make your applications work thus making it easier to identify and handle the security aspects of the application when compared to other applications because there are less points-of-contact to tamper with (but remember: any software is just as secure as you make it; don't go out trying to create the next shell with this and claim I said it would be more secure just because of that).

4. Terminal applications can use any and all functionality provided by the .NET platform, including third-party libraries and communicate with modern remote REST APIs (even providing them while the application is running). The same goes for any resource available on your IDE of choice, like step-by-step debugging. The only requirement is that you/they don't mess with the <code>System.Console</code> class during the lifetime of the application.

5. Also, any application can actually be deployed to production and use by your users and even replace web-based applications (yes, they can) in any context using only SSH -- however I don't recommend using it for external users.

6. Retro is in high nowadays and the console is not going away any fast, so why not.

## Usage

Using the Terminal toolkit is really easy and simple as I outlined. However, I thought it would be best if it was given it's own separate document. You can see [how to start using the Terminal toolkit here](USAGE.md).

## To-Do List

There is still a lot of work to be done on the Terminal toolkit and although I may tackle these issues in the future, any help is appreciated: 

* Enhance masks to support using literals and maybe input alignment.
* Maybe add support for using specific colours not just the highlight colour.
* SSH applications can transfer files, how can ease support for that? How to support that locally?
* Code reviews and optimisations are always welcome.