![Datasilk Logo](http://www.markentingh.com/projects/datasilk/logo.png)

# Datasilk Core Template

This Github repository is a project template for an ASP.NET Core web site based on [Datasilk Core](http://github.com/Datasilk/Core), [Datasilk Core Js](http://github.com/Datasilk/CoreJs), [Selector](http://github.com/websilk/selector), and [Tapestry](http://github.com/Websilk/Tapestry). The website includes a user system, login page, and account dashboard. 

This project is meant to be forked and used as a starting point for developing large-scale, high-performing websites.

## Requirements

* Visual Studio 2017
* ASP.NET Core 2.0
* SQL Server 2016
* Node.js
* Gulp

## Installation

1. Clone the repository:

    ```git clone --recurse-submodules http://github.com/datasilk/coretemplate YourProjectName```

	> NOTE: replace `YourProjectName` with the name of your project

2. Replace all case-sensitive instances of `CoreTemplate` to `YourProjectName` and `coretemplate` to `yourprojectname` in all files within the repository
3. Rename file `CoreTemplate.sln` to `YourProjectName.sln` and file `App/CoreTemplate.csproj` to `App/YourProjectName.csproj`
2. Run command ```npm install```
3. Run command ```gulp default```
4. In Visual Studio, publish the SQL project to SQL Server 2016 (or greater), with your own database name
5. Open `config.json` and make sure the database connection string for property `SqlServerTrusted` points to your database.
6. Click Play in Visual Studio 2017


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
     
#### Example: HTML with Scaffolding variables & blocks
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




