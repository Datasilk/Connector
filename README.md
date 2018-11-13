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
In order to subscribe to a node, you must know the node IP Address or domain name either by browsing root servers for public node lists, or by getting that information from the friend who has an account on the node. Next, you'll need the username of the person/friend you wish to subscribe to. Upon sending your username along with your friend's username via REST to the desired node, a response will be returned with a private key (sent encrypted over HTTPS). That private key will forever be stored, linking your local user account with the remote user on the node server of choice and enabling HMAC+ChaCha20 encryption for any future messages. Once that user accepts your subscription request, you'll be able to securely view their timeline from your node's website portal.


All above projects (including this one) were concieved & developed by [Mark Entingh](https://www.markentingh.com), who has a strong passion for web development.




