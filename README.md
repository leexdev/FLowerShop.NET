# FLowerShop using ASP.NET MVC
Hướng dẫn sử dụng
<br>
**Cài đặt**
<br>
**- Oracle Database 21c Express Edition:** https://www.oracle.com/database/technologies/xe-downloads
<br>
**- SQL Developer:** https://www.oracle.com/database/sqldeveloper/technologies/download/
<br>
**- ODT for Visual Studio:** https://marketplace.visualstudio.com/items?itemName=OracleCorporation.OracleDeveloperToolsForVisualStudio2022
<br>
**Bước 1:**
<br>
- Vào SQL Developer tạo User mới với Username: C##ADMIN và Password: 123 và phân quyền: connect, dba, resource cho user đó.
<br>
- Tạo New Connection với tên là: FLowerShopDB.
<br>
**Bước 2:**
<br>
- Mở chương trình bằng Visual Studio.
<br>
- Mở file Web.config sửa TNS_ADMIN đúng với đường dẫn máy của mình. VD: TNS_ADMIN=C:\Users\LE\Oracle\network\admin.
<br>
**Bước 3**
<br>
- Chạy chương trình.
