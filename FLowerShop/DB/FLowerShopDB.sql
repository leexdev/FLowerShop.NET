CREATE TABLE FlowerTypes (
    flowertype_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    flowertype_name NVARCHAR2(255),
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
);

CREATE TABLE Flowers (
    flower_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    flower_name NVARCHAR2(255),
    flower_image NVARCHAR2(255),
    description CLOB,
    old_price NUMBER,
    new_price NUMBER,
    flowertype_id RAW(16) REFERENCES FlowerTypes(flowertype_id),
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
);

CREATE TABLE Users (
    user_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    user_name NVARCHAR2(255),
    user_email NVARCHAR2(255),
    user_phone NVARCHAR2(15),
    user_password NVARCHAR2(255),
    role NUMBER(1) DEFAULT 0 NOT NULL,
    facebookid varchar2(20),
    resettoken RAW(16),
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
);

CREATE TABLE ShoppingCart (
    cart_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    user_id RAW(16) REFERENCES Users(user_id),
    flower_id RAW(16) REFERENCES Flowers(flower_id),
    quantity NUMBER,
    subtotal NUMBER,
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
);

CREATE TABLE DiscountCodes (
    discount_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    code NVARCHAR2(20),
    discount_type NUMBER(1), 
    discount_value NUMBER,
    minimum_order_amount NUMBER,
    start_date DATE,
    end_date DATE,
    code_count NUMBER,
    description NVARCHAR2(255),
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
);

CREATE TABLE Orders (
    order_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    user_id RAW(16) REFERENCES Users(user_id),
    order_date DATE,
    total_amount NUMBER,
    discount_id RAW(16) REFERENCES DiscountCodes(discount_id),
    message_to_recipient NVARCHAR2(1000), -- L?i nh?n cho ng??i nh?n
    message_to_shop NVARCHAR2(1000), -- L?i nh?n cho shop
    hide_sender_info NUMBER(1), -- ?n thông tin ng??i g?i (0 ho?c 1)
    payment_method NUMBER(1), -- Hình th?c thanh toán
    sender_name NVARCHAR2(255),
    sender_phone NVARCHAR2(15),
    sender_email NVARCHAR2(255),
    recipient_name NVARCHAR2(255),
    recipient_phone NVARCHAR2(15),
    recipient_address CLOB, 
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
);


CREATE TABLE OrderDetails (
    orderdetail_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    order_id RAW(16) REFERENCES Orders(order_id),
    flower_id RAW(16) REFERENCES Flowers(flower_id),
    quantity NUMBER,
    subtotal NUMBER,
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
);

CREATE TABLE OrderHistory (
    history_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    order_id RAW(16) REFERENCES Orders(order_id) NOT NULL,
    change_date DATE,
    content CLOB
);

CREATE TABLE UserDiscount (
    userDiscount_id RAW(16) DEFAULT SYS_GUID() PRIMARY KEY,
    user_id RAW(16) REFERENCES Users(user_id),
    discount_id RAW(16) REFERENCES DiscountCodes(discount_id),
    Deleted NUMBER(1) DEFAULT 0 NOT NULL
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
VALUES ('Bó hoa sắc màu', 'bo-hoa-sinh-nhat-sac-mau-570x570.jpg', 'A beautiful red rose', 330000, 300000, (SELECT flowertype_id FROM FlowerTypes WHERE flowertype_name = 'Hoa chúc mừng'));
INSERT INTO Flowers (flower_name, flower_image, description, old_price, new_price, flowertype_id)
VALUES ('May mắn', 'hoa-chuc-mung-may-man-570x570.jpg', 'A beautiful red rose', null, 590000, (SELECT flowertype_id FROM FlowerTypes WHERE flowertype_name = 'Hoa khai trương'));

INSERT INTO DiscountCodes (code, discount_type, discount_value, minimum_order_amount, start_date, end_date, code_count, description, Deleted)
VALUES ('CODE40', 1, 10, 400000, TO_DATE('2023-01-01', 'YYYY-MM-DD'), TO_DATE('2023-12-31', 'YYYY-MM-DD'), 10, NULL, 0);
INSERT INTO DiscountCodes (code, discount_type, discount_value, minimum_order_amount, start_date, end_date, code_count, description, Deleted)
VALUES ('SALE50', 0, 50000, 600000, TO_DATE('2023-04-15', 'YYYY-MM-DD'), TO_DATE('2023-04-30', 'YYYY-MM-DD'), 20, NULL, 0);
INSERT INTO DiscountCodes (code, discount_type, discount_value, minimum_order_amount, start_date, end_date, code_count, description, Deleted)
VALUES ('VUIVE', 1, 15, 800000,TO_DATE('2023-01-01', 'YYYY-MM-DD'), TO_DATE('2023-12-31', 'YYYY-MM-DD'), 30,NULL, 0);       

