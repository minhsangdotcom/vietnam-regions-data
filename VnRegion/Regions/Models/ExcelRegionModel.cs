using MiniExcelLibs.Attributes;

namespace VnRegion.Regions.Models;

public class ExcelRegionModel
{
    [ExcelColumnName("Tỉnh Thành Phố")]
    public string? Province { get; set; }

    [ExcelColumnName("Mã TP")]
    public string? ProvinceCode { get; set; }

    [ExcelColumnName("Quận Huyện")]
    public string? District { get; set; }

    [ExcelColumnName("Mã QH")]
    public string? DistrictCode { get; set; }

    [ExcelColumnName("Phường Xã")]
    public string? Ward { get; set; }

    [ExcelColumnName("Mã PX")]
    public string? WardCode { get; set; }

    [ExcelColumnName("Cấp")]
    public string? Level { get; set; }

    public ExcelRegionModel Clone()
    {
        return (ExcelRegionModel)MemberwiseClone();
    }
}
