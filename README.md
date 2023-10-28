<!DOCTYPE html>
<html>
<head>
    <title>FLowerShop using ASP.NET MVC - Hướng dẫn sử dụng</title>
</head>
<body>
    <h1>Hướng dẫn sử dụng FLowerShop using ASP.NET MVC</h1>
    
    <h2>Cài đặt</h2>
    <ul>
        <li><a href="https://www.oracle.com/database/technologies/xe-downloads">Oracle Database 21c Express Edition</a></li>
        <li><a href="https://www.oracle.com/database/sqldeveloper/technologies/download/">SQL Developer</a></li>
        <li><a href="https://marketplace.visualstudio.com/items?itemName=OracleCorporation.OracleDeveloperToolsForVisualStudio2022">ODT for Visual Studio</a></li>
    </ul>

    <h2>Bước 1</h2>
    <p>
        - Vào SQL Developer tạo User mới với Username: C##ADMIN và Password: 123 và phân quyền: connect, dba, resource cho user đó.
    </p>
    <p>
        - Tạo New Connection với tên là: FLowerShopDB.
    </p>

    <h2>Bước 2</h2>
    <p>
        - Mở chương trình bằng Visual Studio.
    </p>
    <p>
        - Mở file Web.config sửa TNS_ADMIN đúng với đường dẫn máy của mình. VD: TNS_ADMIN=C:\Users\LE\Oracle\network\admin.
    </p>

    <h2>Bước 3</h2>
    <p>
        - Chạy chương trình.
    </p>
</body>
</html>
