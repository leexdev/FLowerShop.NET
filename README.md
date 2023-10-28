# FLowerShop using ASP.NET MVC
Hướng dẫn sử dụng
<br>
**Cài đặt**
**- Oracle Database 21c Express Edition:** https://www.oracle.com/database/technologies/xe-downloads
**- SQL Developer:** https://www.oracle.com/database/sqldeveloper/technologies/download/
**- ODT for Visual Studio:** https://marketplace.visualstudio.com/items?itemName=OracleCorporation.OracleDeveloperToolsForVisualStudio2022
**Bước 1:**
- Vào SQL Developer tạo User mới với Username: C##ADMIN và Password: 123 và phân quyền: connect, dba, resource cho user đó.
- Tạo New Connection với tên là: FLowerShopDB.
**Bước 2:**
- Mở chương trình bằng Visual Studio.
- Mở file Web.config sửa TNS_ADMIN đúng với đường dẫn máy của mình. VD: TNS_ADMIN=C:\Users\LE\Oracle\network\admin.
**Bước 3**
- Chạy chương trình.