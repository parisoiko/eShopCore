# eShopCore

This is my back-end work on the same eShop project. Essentially all of the functions (etc searches, categories) have moved onto server side, all the back-and-forth communication is happening via AJAX and SignalR. It currently works without a database because I can't figure out how to upload an SQL server database on a git project, so it functions with a simple list. But the code for the dbcontext filtration is there, it just needs proper migrations and database updates before running. 

It also has a CRUD/admin page where you can edit the database client-side. 
