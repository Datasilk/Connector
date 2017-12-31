![Datasilk Logo](http://www.markentingh.com/projects/datasilk/logo.png)

# Core Template
This project is a template ASP.NET Core web site based on [Datasilk Core](http://github.com/Datasilk/Core), [Datasilk Core Js](http://github.com/Datasilk/CoreJs), [Selector](http://github.com/websilk/selector), and [Tapestry](http://github.com/Websilk/Tapestry). The website includes a user system, login page, and account dashboard. 

This project is meant to be forked and used as a starting point for developing large-scale, high-performing websites.


## Features
* **SQL Server project** includes a **Users** `table` and `sequence` script, along with various `stored procedures`
* **Query project** uses [Dapper](http://github.com/StackExchange/Dapper) to populate C# models from SQL
* **App project** uses ASP.NET Core 2.0 and [Datasilk Core](http://github.com/Datasilk/Core) to host web pages & web APIs.
	* When accessing website for the first time, you're able to create a new admin user account
    * Can log in to user account at `/login` or from any secure page when access is denied
    * Redirects to `/dashboard` after user logs into account
    * Default web page for URL `/` is `/Pages/Home/Home.cs`
    * Dashboard contains a sidebar with a menu system
    * UI provided by [Tapestry](http://github.com/Websilk/Tapestry), a **CSS/LESS** UI framework.
    * Javascript uses [Selector](http://github.com/websilk/selector) as a light-weight replacement for jQuery
	* [Datasilk Core Js](http://github.com/Datasilk/CoreJs) is used as a simple **client-side framework** for structuring page-level Javascript code, making AJAX requests, and calling utility functions
	* Build **MVC** web pages using html files & **scaffolding variables**. For example:
     
#### HTML file with Scaffolding variables & blocks
```
<html><body>
	<div class="menu">{{menu}}</div>
	{{has-sub-menu}}
		<div class="menu sub">{{sub-menu}}</div>
	{{/has-sub-menu}}
	<div class="body">{{content}}</div>
</body></html>
```

All above projects were concieved & developed by [Mark Entingh](http://www.markentingh.com), who has a strong passion for web development.




