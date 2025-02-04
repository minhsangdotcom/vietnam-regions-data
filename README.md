# VietNam Regions Database

![Vietnam map](Assets/vietnam-map.jpg)

Project này được tạo ra để thống kê một cách chi tiết về dữ liệu của các tình thành, quận huyện, phường xã thuộc Việt Nam.

Dữ liệu được tạo ra dưới dạng file json và sql, không những da dạng về đầu ra dữ liệu, mà còn cho phép chúng ta tùy chỉnh tốt hơn về table, colum, loại cơ sở dữ liệu.

# Thả :star: cho mình nhé

Nếu các thấy project này hay và hữu ích, hãy cân nhắc cho mình 1 :star: nhé. Đó chính là động lực để mình ra tiếp những project tiếp theo.

# Tổng quan

Dữ liệu được sử dụng cho project được lấy tại [https://danhmuchanhchinh.gso.gov.vn/](https://danhmuchanhchinh.gso.gov.vn/) . Đây là trang web thống kê dữ liệu chính thức của Việt Nam.

Project được viết dưới dạng Web API, gồm có 2 api chính là

```
/api/regions/generateJson
```

Dùng để xuất dữ liệu dưới dạng JSON file.

```
/api/regions/generateSqlQuery
```

Dùng để xuất dữ liệu dưới dạng SQL file.

Đầu ra của file ở thư mục [Outputs](VnRegion/Outputs/)

Một chút preview sơ lượt về SQL file:

```Sql
INSERT INTO province (administrative_unit_id,code,custom_name,english_full_name,english_name,full_name,id,name) VALUES
	('1','01','','Ha Noi City','Ha Noi','Thành phố Hà Nội','01JK3AAK5QVBK9ZXK5TVDM9A8P','Hà Nội'),
	('2','02','','Ha Giang Province','Ha Giang','Tỉnh Hà Giang','01JK3AAK6N8FV2H74J8YJY05TQ','Hà Giang'),
	('2','04','','Cao Bang Province','Cao Bang','Tỉnh Cao Bằng','01JK3AAK6ZDKK09S689J71M9CR','Cao Bằng'),
	('2','06','','Bac Kan Province','Bac Kan','Tỉnh Bắc Kạn','01JK3AAK76EYXQFYD6TWEZW5ZW','Bắc Kạn'),
	('2','08','','Tuyen Quang Province','Tuyen Quang','Tỉnh Tuyên Quang','01JK3AAK7BTRQK88R69C84P3HY','Tuyên Quang'),
	('2','10','','Lao Cai Province','Lao Cai','Tỉnh Lào Cai','01JK3AAK7KX2S8PMXZAQHQ5KD2','Lào Cai'),
	('2','11','','Dien Bien Province','Dien Bien','Tỉnh Điện Biên','01JK3AAK7S82EXXCXYX08HGVEF','Điện Biên'),
	('2','12','','Lai Chau Province','Lai Chau','Tỉnh Lai Châu','01JK3AAK80E2QZQ8P63HJ40SMQ','Lai Châu'),
	('2','14','','Son La Province','Son La','Tỉnh Sơn La','01JK3AAK85JGABP3XDCP5CCYRE','Sơn La'),
	('2','15','','Yen Bai Province','Yen Bai','Tỉnh Yên Bái','01JK3AAK8EA93JB1NVE4BJ0QZK','Yên Bái'),
	('2','17','','Hoa Binh Province','Hoa Binh','Tỉnh Hoà Bình','01JK3AAK8QXYM018HC5YQQ4M9C','Hoà Bình');
```

```sql

INSERT INTO district (administrative_unit_id,code,custom_name,english_full_name,english_name,full_name,id,name,province_code,province_id) VALUES
	('5','001','','Ba Dinh District','Ba Dinh','Quận Ba Đình','01JK3AAKTHM808NDAYTP06T48V','Ba Đình','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','002','','Hoan Kiem District','Hoan Kiem','Quận Hoàn Kiếm','01JK3AAKTKP9SAFTR95V5GHRV4','Hoàn Kiếm','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','003','','Tay Ho District','Tay Ho','Quận Tây Hồ','01JK3AAKTK7D39CZHJ4KYPNMX3','Tây Hồ','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','004','','Long Bien District','Long Bien','Quận Long Biên','01JK3AAKTK1JSHD43BPEN7VSBB','Long Biên','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','005','','Cau Giay District','Cau Giay','Quận Cầu Giấy','01JK3AAKTMVF58G19JMD9XS70J','Cầu Giấy','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','006','','Dong Da District','Dong Da','Quận Đống Đa','01JK3AAKTMX3CQXKBQ5ZYVV0SJ','Đống Đa','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','007','','Hai Ba Trung District','Hai Ba Trung','Quận Hai Bà Trưng','01JK3AAKTNRWX6PYN0W5FR2WP1','Hai Bà Trưng','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','008','','Hoang Mai District','Hoang Mai','Quận Hoàng Mai','01JK3AAKTP9J0XYHBQM5Z62AKZ','Hoàng Mai','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('5','009','','Thanh Xuan District','Thanh Xuan','Quận Thanh Xuân','01JK3AAKTSHB0CGPA80BZB1T6X','Thanh Xuân','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('7','016','','Soc Son District','Soc Son','Huyện Sóc Sơn','01JK3AAKTSJT7V8Q11V0CGSMQV','Sóc Sơn','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('7','017','','Dong Anh District','Dong Anh','Huyện Đông Anh','01JK3AAKTT6X0NMFW8J4J28NKW','Đông Anh','01','01JK3AAK5QVBK9ZXK5TVDM9A8P'),
	('7','018','','Gia Lam District','Gia Lam','Huyện Gia Lâm','01JK3AAKTVJDCXP62FBR8ZVNF0','Gia Lâm','01','01JK3AAK5QVBK9ZXK5TVDM9A8P');
```

```sql
INSERT INTO ward (administrative_unit_id,code,custom_name,district_code,district_id,english_full_name,english_name,full_name,id,name) VALUES
	('8','00001','','001','01JK3AAKTHM808NDAYTP06T48V','Phuc Xa Ward','Phuc Xa','Phường Phúc Xá','01JK3AANF6BZGBH9R3SJXA5KVE','Phúc Xá'),
	('8','00004','','001','01JK3AAKTHM808NDAYTP06T48V','Truc Bach Ward','Truc Bach','Phường Trúc Bạch','01JK3AANF7GP9TTBM3C4H30R44','Trúc Bạch'),
	('8','00006','','001','01JK3AAKTHM808NDAYTP06T48V','Vinh Phuc Ward','Vinh Phuc','Phường Vĩnh Phúc','01JK3AANF725S2SH1RG1C0MZN5','Vĩnh Phúc'),
	('8','00007','','001','01JK3AAKTHM808NDAYTP06T48V','Cong Vi Ward','Cong Vi','Phường Cống Vị','01JK3AANF78QZPPT56QEP66TS1','Cống Vị');
```

Một chút preview sơ lượt về JSON file:

API Output

```json
{
  "sqlGenerationPath": null,
  "updateMetaData": null,
  "provinceMetaData": {
    "path": "\\Outputs\\Provinces.json",
    "total": 63
  },
  "districtMetaData": {
    "path": "\\Outputs\\Districts.json",
    "total": 701
  },
  "wardMetaData": {
    "path": "\\Outputs\\Wards.json",
    "total": 10333
  },
  "administrativeUnitData": {
    "path": "\\Outputs\\AdministrativeUnits.json",
    "total": 10
  }
}
```

file Provinces.json

```json
[
  {
    "Id": "01JK3E21PZ1K2TV19QZA77NYCM",
    "Code": "01",
    "Name": "Hà Nội",
    "EnglishName": "Ha Noi",
    "FullName": "Thành phố Hà Nội",
    "EnglishFullName": "Ha Noi City",
    "AdministrativeUnitId": 1
  },
  {
    "Id": "01JK3E21QWGJMK20TWNAAZCHZG",
    "Code": "02",
    "Name": "Hà Giang",
    "EnglishName": "Ha Giang",
    "FullName": "Tỉnh Hà Giang",
    "EnglishFullName": "Ha Giang Province",
    "AdministrativeUnitId": 2
  },
  {
    "Id": "01JK3E21R6FJTZKTCG1NXDYY1S",
    "Code": "04",
    "Name": "Cao Bằng",
    "EnglishName": "Cao Bang",
    "FullName": "Tỉnh Cao Bằng",
    "EnglishFullName": "Cao Bang Province",
    "AdministrativeUnitId": 2
  },
   ..........
]
```

file Districts.json

```json
[
  {
    "ProvinceCode": "01",
    "ProvinceId": "01JK3E21PZ1K2TV19QZA77NYCM",
    "Id": "01JK3E228GZB523QBWCPJH2ZFD",
    "Code": "001",
    "Name": "Ba Đình",
    "EnglishName": "Ba Dinh",
    "FullName": "Quận Ba Đình",
    "EnglishFullName": "Ba Dinh District",
    "AdministrativeUnitId": 5
  },
  {
    "ProvinceCode": "01",
    "ProvinceId": "01JK3E21PZ1K2TV19QZA77NYCM",
    "Id": "01JK3E228JGZ2C828A82ZJ57NX",
    "Code": "002",
    "Name": "Hoàn Kiếm",
    "EnglishName": "Hoan Kiem",
    "FullName": "Quận Hoàn Kiếm",
    "EnglishFullName": "Hoan Kiem District",
    "AdministrativeUnitId": 5
  },
  {
    "ProvinceCode": "01",
    "ProvinceId": "01JK3E21PZ1K2TV19QZA77NYCM",
    "Id": "01JK3E228KP5P2PYEMQVRVZFS9",
    "Code": "003",
    "Name": "Tây Hồ",
    "EnglishName": "Tay Ho",
    "FullName": "Quận Tây Hồ",
    "EnglishFullName": "Tay Ho District",
    "AdministrativeUnitId": 5
  }
  .....
]
```

File Ward.json

```json
[
  {
    "DistrictCode": "001",
    "DistrictId": "01JK3E228GZB523QBWCPJH2ZFD",
    "Id": "01JK3E22R82BMJ5XK263WRF30V",
    "Code": "00001",
    "Name": "Phúc Xá",
    "EnglishName": "Phuc Xa",
    "FullName": "Phường Phúc Xá",
    "EnglishFullName": "Phuc Xa Ward",
    "AdministrativeUnitId": 8
  },
  {
    "DistrictCode": "001",
    "DistrictId": "01JK3E228GZB523QBWCPJH2ZFD",
    "Id": "01JK3E22R9T5NH058J7XYMAT8D",
    "Code": "00004",
    "Name": "Trúc Bạch",
    "EnglishName": "Truc Bach",
    "FullName": "Phường Trúc Bạch",
    "EnglishFullName": "Truc Bach Ward",
    "AdministrativeUnitId": 8
  }
]
```

**_Lưu ý : Dữ liệu chỉ áp dụng cho relation database, đã test trên PostgreSQL, SQL Server, MYSQL_**

# Bắt đầu nào :rocket:

## Cấu trúc database

![Schema](/Assets/schema.png)

### Cài đặt ⚙️

Bước 1 : Clone project.

Bước 2 : Thực thi câu lệnh sau để chạy project

```
cd VietNamRegion
dotnet run
```

Bước 3 : Đi tới thư mục Database và chạy file create_table.sql để tạo cấu trúc dữ liệu.

### Hướng dẫn sử dụng

Để có được file excel input thì mọi người truy cập vào [website](https://danhmuchanhchinh.gso.gov.vn/) hoặc mình có để ở thư mục [Inputs](/VnRegion/Inputs/).

Lưu ý là chọn option để xuất file excel bao gồm tỉnh thành, quận huyện, phường xã nha.

File xuất ra sẽ có dạng xls mà mình thì chỉ nhận file xlsx thôi nên phải convert nó về đúng định dạng, truy cập vào [đây](https://www.freeconvert.com/document-converter) để định dạng.

Sau đó mọi người muốn dữ liệu được generate ra dưới dạng gì thì chọn API tương ứng :sunglasses:.

Option 1 : Chọn xuất file dưới dạng JSON file rồi seed data, cách này mình đã áp dụng vào dự án thực tế rồi chạy rất OK :ok_hand: :smile:

![Screenshot](/Assets/screenshot1.png)

**_file_** : là file dữ liệu hiện tại.

**_updatedFile_** : là file dữ liệu mới.

Để cho dễ hình dung mình sẽ đưa ra ví dụ như sau : Với thòi điểm hiện tại mình có được file excel A, mình sẽ upload file A vào file input trong API, nhưng nếu như sau này 2-3 năm nữa có nghị quyết mới, dữ liệu được cập nhật mình sẽ có dược file excel B.

Nhưng trong csdl thì vẫn là dữ liệu file A để cập nhật dữ liệu mới mình làm như sau.

Lấy ra file A( dữ liệu cũ) upload vào file input, file B( dữ liệu mới) upload vào updatedFile input.

Thì API sẽ xuất ra một file có tên là Changes.json, nó sẽ ghi lại tất cả các thay đổi cho mọi người cập nhật cũng như là preview.

_Lưu ý cách này áp dụng cho hình thức seed data trong c# đặc biệt là ASP.NET CORE mọi người có thể tham khảo ở [đây](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding#custom-initialization-logic)_

File Changes.json sẽ có dạng

```json
{
  "wardChanges": [],
  "districtChanges": [],
  "provinceChanges": []
}
```

**_wardChanges_**: danh sách thay đổi của phường xã.

**_districtChanges_**: danh sách thay đổi của quận huyện.

**_provinceChanges_**: danh sách thay đổi của tỉnh thành.

Dữ liệu thay đổi sẽ có cấu trúc như sau

```json
{
      "code": "32002",
      "old": {
        "code": "32002",
        "name": "4",
        "englishName": "4",
        "fullName": "Phường 4",
        "englishFullName": "Ward 4",
        "districtCode": "964",
        "customName": "Phường 4",
        "administrativeUnitId": 8
      },
      "new": {
        "code": "32002",
        "name": "2",
        "englishName": "2",
        "fullName": "Phường 2",
        "englishFullName": "Ward 2",
        "districtCode": "964",
        "customName": "Phường 2",
        "administrativeUnitId": 8
      },
      "changes": [
        {
          "property": "Ward",
          "old": "Phường 4",
          "current": "Phường 2"
        }
      ],
      "update": {
        "code": "32002",
        "name": "2",
        "englishName": "2",
        "fullName": "Phường 2",
        "englishFullName": "Ward 2",
        "districtCode": "964",
        "customName": "Phường 2",
        "administrativeUnitId": 8
      },
      "type": 2
    },
```

**_code_** : mã vùng.

**_old_** : Dữ liệu cũ

**_new_** : Dữ liệu mới

**_changes_** : Thay đổi giữa dữ liệu cũ và mới.

**_update_** : Dùng để thực thiện các logic update dữ liệu khi seed data.

**_type_** : loại thay đổi với 1 là thêm mới, 2 là cập nhật, 3 là xóa

Với vd là đoạn json trên thì phường (xã , thị trấn) có mã là 32002 có những thay đổi là từ phường 4 đổi thành phường 2.

_Lưu ý là khi thực hiện logic cập nhật thì mọi người dùng changes property để update lại thay đổi nhé, những cái khác chỉ để mọi người preview thôi, mà nhớ là dùng code để tìm record thay vì id nhé. Với những quận,huyện đổi tình(thành phố) thì các bạn chịu khó dùng province code để tìm province id sau đó update lại khóa ngoại nhé, hoặc để đơn giản thì dùng code làm khóa chính thì sẽ dễ dàng hơn._

Option 2 : Xuất file dữ liệu dưới dạng Sql file.

![screenshot](/Assets/screenshot2.png)

Ở API này chỉ cho update 1 file, chứ chưa hỗ trợ tự động cập nhật sự thay đổi như API Json file.

### Tùy chỉnh

Tùy chỉnh được thiết lập ở phần appsettings.Development.json.

Tùy chỉnh loại CSDL.

```json
"DbSetting": "SqlServer",
```

Những loại cở sở dữ liệu được hỗ trợ:

| Loại CSDL  |
| ---------- |
| Mysql      |
| PostgreSql |
| SqlServer  |
| OracleSql  |

**_Tùy chỉnh loại CSDL chỉ áp dụng cho xuất file dạng sql._**

Tùy chỉnh tên table hoặc tên tên columns.

```json
"ProvinceConfigs": {
    "TableName": "province",
    "ColumnNames": {
      "Id": "Id",
      "Code": "Code",
      "Name": "Name",
      "EnglishName": "EnglishName",
      "FullName": "FullName",
      "EnglishFullName": "EnglishFullName",
      "CustomName": "CustomName",
      "AdministrativeUnitId": "AdministrativeUnitId"
    }
  },
```

**_TableName_**: Tùy chỉnh cho tên table trong vd là province

**_ColumnNames_**: Tùy chỉnh cho tên column

Khi tùy chỉnh tên cột thì chỉ cần chỉnh lại value thôi nhé còn key thì giữ nguyên.

VD: Muốn tùy chỉnh tên tiếng anh của tỉnh(thành phố) thành "EnName" thì chỉ cần sửa lại value của trường EnglishName.

```json
"ColumnNames": {
    "Id": "Id",
    "Code": "Code",
    "Name": "Name",
    "EnglishName": "EnName",
    "FullName": "FullName",
    "EnglishFullName": "EnglishFullName",
    "CustomName": "CustomName",
    "AdministrativeUnitId": "AdministrativeUnitId"
  }
```
# Lưu ý

Có 2 option cho các bạn lựa chọn là tùy chỉnh tên của table, column trong appsettings nếu appsettings trống tự động lấy tên mặc định của entity ở thư mục [Entities](/VnRegion/Regions/Entities/).

Nhưng để đạt được tốc độ tối đa tầm khoảng 2s thì nên chọn tùy chỉnh trong appsettings nhé vì đọc dữ liệu trong appsettings sẽ nhanh hơn dùng reflection đọc dữ liệu ở class.

Phần tùy chỉnh CSDL bằng  DbSetting property thì là bắc buộc nhé, Nếu trong appsettings trống sẽ báo lỗi 500 trước khi chạy.


# Speed test

Xuất file Json với API /api/regions/generateJson trong vòng 2 giây đo bằng StopWatch

![speed test json api](/Assets/speedtest-json.png)

Xuất file SQL với API /api/regions/generateSqlQuery trong vòng 2 giây đo bằng StopWatch

![speed test sql api](/Assets/speedtest-sql.png)

# Purpose

Mục đích chình mình làm dự án này để giúp mọi người dể dàng lấy được dữ liệu của Tỉnh thành,quận huyện, phường xã để không phải mất công đi cào dữ liệu hoặc sử dụng Web Scraping.

Có rất nhiều project tương tự nhưng bằng rất nhìu ngôn ngữ lập trình khác nhau, nhưng viết bằng C# thì chưa có, nên mình muốn làm một cái gì đó cho cộng đồng C# và những anh em quen thuộc với C# hơn.

# License

Dự án này sử dụng [MIT License](/LICENSE)
