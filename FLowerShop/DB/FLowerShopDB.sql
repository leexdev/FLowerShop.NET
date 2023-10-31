CREATE TABLE FlowerTypes (
    flowertype_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    flowertype_name NVARCHAR2(255),
    Deleted NUMBER(1) DEFAULT 0
);

CREATE TABLE Flowers (
    flower_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    flower_name NVARCHAR2(255),
    flower_image NVARCHAR2(255),
    description CLOB,
    old_price NUMBER,
    new_price NUMBER,
    Deleted NUMBER(1) DEFAULT 0,
    flowertype_id RAW(16) DEFAULT SYS_GUID() REFERENCES FlowerTypes(flowertype_id) NOT NULL
);

CREATE TABLE Users (
    user_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    user_name NVARCHAR2(255),
    user_email NVARCHAR2(255),
    user_password NVARCHAR2(255),
    Deleted NUMBER(1) DEFAULT 0
);

CREATE TABLE ShoppingCart (
    cart_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    user_id RAW(16) DEFAULT SYS_GUID() REFERENCES Users(user_id) NOT NULL,
    flower_id RAW(16) DEFAULT SYS_GUID() REFERENCES Flowers(flower_id) NOT NULL,
    quantity NUMBER,
    subtotal NUMBER,
    Deleted NUMBER(1) DEFAULT 0
);

CREATE TABLE DiscountCodes (
    discount_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    code NVARCHAR2(20),
    discount_type NVARCHAR2(20), --Lo?i gi?m giá (ví d?: ph?n tr?m, s? ti?n c? ??nh).
    discount_value NUMBER, --Giá tr? gi?m giá (ví d?: 10% ho?c 50,000 VN?).
    start_date DATE,
    end_date DATE,
    description NVARCHAR2(255), --Mô t? v? mã gi?m giá (ví d?: "Gi?m 10% cho ??n hàng trên 500,000 VN?")
    Deleted NUMBER(1) DEFAULT 0
);

CREATE TABLE Orders (
    order_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    user_id RAW(16) DEFAULT SYS_GUID() REFERENCES Users(user_id) NOT NULL,
    order_date DATE,
    total_amount NUMBER(10, 2),
    discount_id RAW(16) DEFAULT SYS_GUID() REFERENCES DiscountCodes(discount_id) NOT NULL,
    message_to_recipient NVARCHAR2(1000), -- L?i nh?n cho ng??i nh?n
    message_to_shop NVARCHAR2(1000), -- L?i nh?n cho shop
    hide_sender_info NUMBER(1), -- ?n thông tin ng??i g?i (0 ho?c 1)
    payment_method NVARCHAR2(50), -- Hình th?c thanh toán
    is_paid NUMBER(1), -- Tr?ng thái ?ã thanh toán (0 ho?c 1)
    recipient_name NVARCHAR2(255),
    recipient_phone NVARCHAR2(15),
    recipient_address CLOB,
    Deleted NUMBER(1) DEFAULT 0,
    cart_id RAW(16) DEFAULT SYS_GUID() REFERENCES ShoppingCart(cart_id) NOT NULL
);

CREATE TABLE OrderDetails (
    order_id RAW(16) DEFAULT SYS_GUID() REFERENCES Orders(order_id) NOT NULL,
    flower_id RAW(16) DEFAULT SYS_GUID() REFERENCES Flowers(flower_id) NOT NULL,
    quantity NUMBER,
    subtotal NUMBER,
    Deleted NUMBER(1) DEFAULT 0
);

CREATE TABLE OrderHistory (
    history_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    order_id RAW(16) REFERENCES Orders(order_id) NOT NULL,
    change_date DATE,
    content CLOB,
    Deleted NUMBER(1) DEFAULT 0
);

select * from flowertypes;
select * from flowers;


delete from flowers;
delete from flowertypes;

drop table orderhistory;
drop table  orderdetails;
drop table  orders;
drop table  discountcodes;
drop table  shoppingcart;
drop table  users;
drop table  flowers;
drop table  flowertypes;

INSERT INTO FlowerTypes (flowertype_name) VALUES ('Hoa sinh nhật');
INSERT INTO FlowerTypes (flowertype_name) VALUES ('Hoa khai trương');
INSERT INTO FlowerTypes (flowertype_name) VALUES ('Hoa chúc mừng');
INSERT INTO FlowerTypes (flowertype_name) VALUES ('Hoa tình yêu');

INSERT INTO Flowers (flower_name, flower_image, description, old_price, new_price, flowertype_id)
VALUES ('Bó hoa sắc màu', 'bo-hoa-sinh-nhat-sac-mau-570x570.jpg', 'A beautiful red rose', 10.99, 8.99, (SELECT flowertype_id FROM FlowerTypes WHERE flowertype_name = 'Hoa chúc mừng'));
INSERT INTO Flowers (flower_name, flower_image, description, old_price, new_price, flowertype_id)
VALUES ('May mắn', 'hoa-chuc-mung-may-man-570x570.jpg', 'A beautiful red rose', null, 590.000, (SELECT flowertype_id FROM FlowerTypes WHERE flowertype_name = 'Hoa khai trương'));

INSERT INTO DiscountCodes (code, discount_type, discount_value, start_date, end_date, description, Deleted)
VALUES ('CODE123', 'Percentage', 10, TO_DATE('2023-01-01', 'YYYY-MM-DD'), TO_DATE('2023-12-31', 'YYYY-MM-DD'), 'Giảm 10% cho đơn hàng trên 500,000 VNĐ', 0);
INSERT INTO DiscountCodes (code, discount_type, discount_value, start_date, end_date, description, Deleted)
VALUES ('SALE50', 'Fixed', 50000, TO_DATE('2023-04-15', 'YYYY-MM-DD'), TO_DATE('2023-04-30', 'YYYY-MM-DD'), 'Giảm 50,000 VNĐ cho đơn hàng bất kỳ', 0);
INSERT INTO DiscountCodes (code, discount_type, discount_value, start_date, end_date, description, Deleted)
VALUES ('NEWUSER', 'Percentage', 15, TO_DATE('2023-01-01', 'YYYY-MM-DD'), TO_DATE('2023-12-31', 'YYYY-MM-DD'), 'Giảm 15% cho người dùng mới', 0);

SELECT * FROM NLS_DATABASE_PARAMETERS WHERE PARAMETER LIKE 'NLS_%CHARACTERSET';

