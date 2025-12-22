using Backend.Dto;

namespace Backend.ViewModel;

public class FormViewModel
{
    public FormViewModel(FormDetailDto dto)
    {
        this.FullName = dto.AnswersDto.FullName;
        this.Department = dto.AnswersDto.Department;
        this.Email = dto.AnswersDto.Email;
        this.IntendedMajor= dto.AnswersDto.IntendedMajor;
        this.PhoneOrLine = dto.AnswersDto.PhoneOrLine;
        this.School = dto.AnswersDto.School;
        this.TargetCountry = dto.AnswersDto.TargetCountry;
        this.ProgramType = dto.AnswersDto.ProgramType;
        this.DepartYear = dto.AnswersDto.DepartYear;
        this.Referral = dto.AnswersDto.Referral;
        this.OtherInfo = dto.AnswersDto.OtherInfo;
        this.QuestionToResolve = dto.AnswersDto.QuestionToResolve;
        this.CreatedAt = dto.CreatedAt;
    }
    
    /// <summary>
    /// 中文全名（如：王小明）
    /// </summary>
    public string FullName { get; set; } 

    /// <summary>
    /// 電子郵件（可作為聯絡方式）
    /// </summary>
    public string Email { get; set; } 

    /// <summary>
    /// 電話號碼或 LINE ID（至少需提供一種聯絡方式）
    /// </summary>
    public string PhoneOrLine { get; set; } 

    /// <summary>
    /// 畢業或就讀學校（例如：台灣大學）
    /// </summary>
    public string School { get; set; } 

    /// <summary>
    /// 畢業或就讀科系（例如：資訊工程）
    /// </summary>
    public string Department { get; set; } 

    /// <summary>
    /// 有意願前往的留學國家（例如：美國、英國）
    /// </summary>
    public string TargetCountry { get; set; }

    /// <summary>
    /// 想申請的課程類型（例如：學士、碩士、語言學校）
    /// </summary>
    public string ProgramType { get; set; }

    /// <summary>
    /// 欲就讀的專業或領域（例如：人工智慧、行銷）
    /// </summary>
    public string[] IntendedMajor { get; set; } 

    /// <summary>
    /// 預計出發或入學的年份（例如：2026）
    /// </summary>
    public string DepartYear { get; set; } 

    /// <summary>
    /// 使用者得知我們的來源（例如：IG 廣告、朋友介紹）
    /// </summary>
    public string Referral { get; set; } 

    /// <summary>
    /// 其他補充說明（可空白，用來填寫特殊狀況或需求）
    /// </summary>
    public string OtherInfo { get; set; } 
    
    /// <summary>
    /// 主要想解決的問題
    /// </summary>
    public string[] QuestionToResolve { get; set; }
    
    /// <summary>
    /// 新增時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}