# CoreBlazorSite
Example using .Net Core Blazor to deliver Dynamic responsive content from a Web API

This repository contains key example files that demonstrates how this Azure Hosted website  - https://fundxwebblzreg.azurewebsites.net/ - operates.
        
The site is built with:

- .net Core Blazor
-  Bootstrap
- .Net Web API
-  SQL Server

The Blazor files implement a responsive Bootstrap design that uses a web api to bring back content dynamically.

Blazor was chosen as we wanted a re-usable framework to rapidly build multiple websites. Having re-usable components was very compelling coupled with the opportunity to code back to front in the same language (a programmers Nirvana), virtually eliminating the need to use javascript. I say virtually as you can't quite do everything in Blazor yet. In a recent site I built I had to fall back on jquery to automatically scroll a text area to the bottom. This is relatively easy to achieve via the Blazor Javascipt interop.

The reusable nature of components in Blazor has already spawned a series of open source or paid for component libraries that furhter enhances the rapid bolt together nature of Blazor. I have implemented the open source Blazorise component library with this site. 

The web api was developed in VB.Net and the controller and model source code used to deliver data is also included.

The API calls trigger SQL Server Stored Procedures that implement often complex data queries that range across 80gb of data stored in multiple databases. A complex stored procedure is again also included. The database and production processes were built by myself and a colleague. Daily updates are triggered by rss feeds.

The site implements a modified version of Microsoft's Indentity Model to provide authentication via standard registration and login procedures. The Entity Framework is used for database operations, although often fall back on ADO in my code as I know the protocols well. The Entity Framework connects to a SQL Database.

