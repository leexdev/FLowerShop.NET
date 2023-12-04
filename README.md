# How to Use FlowerShop ASP.NET MVC using Oracle Database

## Installation
- [Oracle Database 21c Express Edition](https://www.oracle.com/database/technologies/xe-downloads.html)
- [SQL Developer](https://www.oracle.com/database/sqldeveloper/technologies/download/)
- [ODT for Visual Studio](https://marketplace.visualstudio.com/items?itemName=OracleCorporation.OracleDeveloperToolsForVisualStudio2022)

## Step 1
- Open SQL Developer and create a new user with Username: C##ADMIN and Password: 123, granting connect, dba, and resource privileges to this user.
- Create a New Connection with the name: FlowerShopDB and log in using the Username and Password you just created.
- Execute the FlowerShopDB.sql file using SQL Developer.

## Step 2
- Open the program in Visual Studio.
- Open the Web.config file and update the TNS_ADMIN to match your machine's path. For example, TNS_ADMIN=C:\Users\LE\Oracle\network\admin.

## Step 3
- Run the program.
