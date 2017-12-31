![Datasilk Logo](http://www.markentingh.com/projects/datasilk/logo.png)

# Core Template
This project is a template ASP.NET Core web site based on [Datasilk Core](http://github.com/Datasilk/Core), [Datasilk Core Js](http://github.com/Datasilk/CoreJs), [Selector](http://github.com/websilk/selector), and [Tapestry](http://github.com/Websilk/Tapestry).


## Features
* **SQL Server project** includes a **Users** `table` and `sequence` script, along with various `stored procedures`
* **Query project** uses [Dapper](http://github.com/StackExchange/Dapper) to populate C# models from SQL
* **App project** uses ASP.NET Core 2.0 and [Datasilk Core](http://github.com/Datasilk/Core) to host web pages & web APIs.
	* Loads a web form to create a new admin user account when database is empty
    * Can log in to user account at `/login` or `/dashboard` or at any secure page when access is denied
    * Redirects to `/dashboard` after user logs into their account
    * Default web page for URL `/` is `/Pages/Home/Home.cs`
    * Dashboard contains a sidebar with a menu system
    * UI provided by [Tapestry](http://github.com/Websilk/Tapestry), a CSS/LESS UI framework.
    * Javascript uses [Selector](http://github.com/websilk/selector) as a light-weight replacement for jQuery
	* [Datasilk Core Js](http://github.com/Datasilk/CoreJs) is used as a simple client-side framework for structuring page-level Javascript code, making AJAX requests, and calling utility functions

All above projects were concieved & developed by [Mark Entingh](http://www.markentingh.com), who has a strong passion for web development.




