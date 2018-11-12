![Datasilk Logo](http://www.markentingh.com/projects/connector/logo.png)

# Connector

An open-source, decentralized, encrypted P2P social networking platform built using ASP.NET Core.

## Requirements

* Visual Studio 2017
* ASP.NET Core 2.0
* SQL Server 2016
* Node.js
* Gulp

## Installation

1. Clone the repository:

    ```git clone --recurse-submodules https://github.com/datasilk/connector```

2. Run command ```npm install```
3. Run command ```gulp default```
4. In Visual Studio, publish the SQL project to SQL Server 2017 (or greater), with your own database name
5. Open `config.json` and make sure the database connection string for property `SqlServerTrusted` points to your database.
6. Click Play in Visual Studio 2017

## Features
* Runs an ASP.NET Core web server that also acts as a **node server**
* Can connect to various **root servers** to retrieve a list of **public** node servers that you can subscribe to
* Can **subscribe** directly to a **private** node server that belongs to a close friend or relative
* Upon logging into your privately owned node server, it will retrieve updated content from subscribed node servers.
* Uses ChaCha20 encryption to protect all data shared across nodes
* Developed upon the [Datasilk Core Template](https://www.github.com/Datasilk/CoreTemplate) MVC project

## FAQ

#### How does subscribing to other node servers work?
This topic is currently open for discussion since it hasn't been solidified how to subscribe securely.


All above projects (including this one) were concieved & developed by [Mark Entingh](https://www.markentingh.com), who has a strong passion for web development.




