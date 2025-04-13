using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Microsoft.VisualBasic; // InputBox 사용
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using UiDesktopApp2.Interfaces;
using UiDesktopApp2.Models;
using Wpf.Ui.Controls;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text.Json;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Reflection;

namespace UiDesktopApp2.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {

        #region FIELDS

        private bool isInitialized = false;

        private readonly ISalary<EsolutionPopulation> salaryService; // ISalary 인터페이스 선언
        private readonly IDatabase<EsolutionPopulation> databaseService; // IDatabase 인터페이스 선언

        #endregion
        #region PROPERTIES
      
        // EsolutionPopulation 리스트  
        [ObservableProperty]
        private IEnumerable<EsolutionPopulation> esolutionPopulations; //ViewModel과 View를 연결하는 ObservableCollection<EsolutionPopulation> 속성

        // 선택된 Employee 데이터 (바인딩용)
        [ObservableProperty]
        private IEnumerable<String?>? employeeNumber;

        [ObservableProperty]
        private int? selectedEmployeeNumber;

        [ObservableProperty]
        private string? selectedName;

        [ObservableProperty]
        private string? selectedDepartment;

        [ObservableProperty]
        private string? selectedPosition;

        [ObservableProperty]
        private string? selectedDateS;

        [ObservableProperty]
        private string? selectedDateE;

        [ObservableProperty]
        private int? selectedBasePay;

        [ObservableProperty]
        private int? selectedRDExpenses;

        [ObservableProperty]
        private int? selectedOvertimePay;

        [ObservableProperty]
        private int? selectedPerformancePay;

        [ObservableProperty]
        private int? selectedMealExpenses;

        [ObservableProperty]
        private int? selectedSelfDrivingSubsidy;

        [ObservableProperty]
        private int? selectedIncomeTax;

        [ObservableProperty]
        private int? selectedLongTermCareInsurance;

        [ObservableProperty]
        private int? selectedLocalIncomeTax;

        [ObservableProperty]
        private int? selectedEmploymentInsurance;

        [ObservableProperty]
        private int? selectedNationalPensionPlan;

        [ObservableProperty]
        private int? selectedEmploymentInsuranceYearEndSettlement;

        [ObservableProperty]
        private int? selectedHealthInsurance;

        [ObservableProperty]
        private int? selectedNationalPensionRetroactive;

        [ObservableProperty]
        private int? selectedTotalPayment;

        [ObservableProperty]
        private int? selectedTotalDeductions;

        [ObservableProperty]
        private int? selectedLoanPayment;

        [ObservableProperty]
        private int? selectedLastPaymentDate;

        // ................................................. 추가 필드 

        [ObservableProperty]
        private int? selectedId;

        [ObservableProperty]
        private IEnumerable<EsolutionPopulation>? filteredResults; // 필터링된 결과

        [ObservableProperty]
        private string? searchCriteria; // 검색 조건

        [ObservableProperty]
        private string? statusMessage; // 상태 메시지

        [ObservableProperty]
        private EsolutionPopulation? selectedEntity; // 선택된 데이터



        // ..............................................................

        #endregion

        #region CONSTRUCTOR

        public DashboardViewModel(ISalary<EsolutionPopulation> salaryService, IDatabase<EsolutionPopulation> databaseService)
        // DI 컨테이너 에서 ISalary<EsolutionPopulation>과 IDatabase<EsolutionPopulation>을 주입받음
        {
            this.salaryService = salaryService ?? throw new ArgumentNullException(nameof(salaryService));
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.esolutionPopulations = new ObservableCollection<EsolutionPopulation>();
            this.filteredResults = new List<EsolutionPopulation>();
            statusMessage = "Ready";
           
            // 초기 데이터 로드
            _ = RefreshDataAsync();

            //// JSON 파일에서 동적 컬럼 정의 로드
            //LoadDynamicColumnDefinitions();
        }

        #endregion

        #region COMMAND

        [RelayCommand]
        private void ReadAllData()
        {
            try
            {
                // 데이터베이스에서 데이터를 동기식으로 가져옵니다.
                this.EsolutionPopulations = this.databaseService?.Get();

                // 삭제된 항목이 제거된 최신 데이터로 EmployeeNumber 컬렉션을 재할당합니다.
                this.EmployeeNumber = this.EsolutionPopulations?
                                        .Select(c => c.EmployeeNumber.ToString())
                                        .ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = $"데이터 로드 중 오류 발생: {ex.Message}";
            }

        }


        [RelayCommand]
        private void DeleteData()
        {
            if (!SelectedEmployeeNumber.HasValue || SelectedEmployeeNumber.Value == 0)
            {
                System.Windows.MessageBox.Show( "삭제할 데이터를 선택하세요." );
                return;
            }

            var result = System.Windows.MessageBox.Show("선택된 데이터를 삭제하시겠습니까 ?", "데이터 삭제", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes) // 사용자가 Yes를 클릭하면 데이터 삭제
            {
                try
                {
                    // 데이터베이스에서 선택된 직원 번호에 해당하는 데이터를 삭제합니다.
                    databaseService.Delete(SelectedEmployeeNumber.Value);

                    // 삭제 후 데이터 재조회하는 메서드 실행 및 콤보박스 데이터 업데이트
                    ReadAllData();

                    // 선택된 값 초기화
                    SelectedEmployeeNumber = null;

                    StatusMessage = "데이터가 성공적으로 삭제되었습니다.";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"데이터 삭제 중 오류 발생: {ex.Message}";
                }
            }
        }



        [RelayCommand]
        private void ReadDetail()
        {
            // 선택된 Employee 데이터를 가져옴
            var targetData = this.databaseService?.GetDetail(this.SelectedEmployeeNumber);

            // ViewModel 속성에 데이터 매핑
            this.SelectedName = targetData.Name;
            this.SelectedDepartment = targetData.Department;
            this.SelectedPosition = targetData.Position;
            this.SelectedDateS = targetData.DateS;
            this.SelectedDateE = targetData.DateE;
            this.SelectedBasePay = targetData.Basepay;
            this.SelectedRDExpenses = targetData.RDExpenses;
            this.SelectedOvertimePay = targetData.OvertimePay;
            this.SelectedPerformancePay = targetData.PerformancePay;
            this.SelectedMealExpenses = targetData.MealExpenses;
            this.SelectedSelfDrivingSubsidy = targetData.SelfDrivingSubsidy;
            this.SelectedIncomeTax = targetData.IncomeTax;
            this.SelectedLongTermCareInsurance = targetData.LongTermCareInsurance;
            this.SelectedLocalIncomeTax = targetData.LocalIncomeTax;
            this.SelectedEmploymentInsurance = targetData.EmploymentInsurance;
            this.SelectedNationalPensionPlan = targetData.NationalPensionPlan;
            this.SelectedEmploymentInsuranceYearEndSettlement = targetData.EmploymentInsuranceYearEndSettlement;
            this.SelectedHealthInsurance = targetData.HealthInsurance;
            this.SelectedNationalPensionRetroactive = targetData.NationalPensionRetroactive;
            this.SelectedTotalPayment = targetData.TotalPayment;
            this.SelectedTotalDeductions = targetData.TotalDeductions;
            this.SelectedLoanPayment = targetData.LoanPayment;
            this.SelectedLastPaymentDate = targetData.LastPaymentDate;
        }
        #endregion

        #region METHOS
        public void OnNavigatedFrom()
        {
            //
        }

        public async void OnNavigatedTo()
        {
            if (!isInitialized)
            {
                await InitializeViewModelAsync();
                isInitialized = true;
            }
        }

        private async Task InitializeViewModelAsync()
        {
            try
            {
                // 데이터를 비동기로 가져오기 (IDatabase 사용)
                this.EsolutionPopulations = await Task.Run(() => this.databaseService?.Get());
                this.EmployeeNumber = this.EsolutionPopulations?.Select(c => c.EmployeeNumber.ToString());
                Console.WriteLine("데이터가 성공적으로 로드되었습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 로드 중 오류 발생: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            try
            {
                // 올바른 DbContext 클래스 이름 사용 (EsolutionDatabaseContext)
                using (var newContext = new EsolutionDatabaseContext())
                {
                    // 데이터베이스에서 최신 데이터를 AsNoTracking()으로 비동기로 불러옵니다.
                    var dataFromDatabase = await newContext.EsolutionPopulations
                                                           .AsNoTracking()
                                                           .ToListAsync();

                    // EmployeeNumber가 0이면 미저장 데이터로 간주하여 필터링
                    var validData = dataFromDatabase.Where(d => d.EmployeeNumber != 0).ToList();

                    // ObservableCollection 업데이트하여 UI에 바인딩
                    EsolutionPopulations = new ObservableCollection<EsolutionPopulation>(validData);
                }

                // 상태 메시지 업데이트
                StatusMessage = $"{EsolutionPopulations.Count()}개의 데이터를 새로고침했습니다.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"새로고침 중 오류 발생: {ex.Message}";
            }
        }

        // update & get 활용 ,, 방안 생각 enum 상수
        [RelayCommand]
        private async Task SaveDataAsync()
        {
            try
            {
                // 전체 저장 대상이 되는 컬렉션이 비어있다면
                if (EsolutionPopulations == null)
                {
                    StatusMessage = "저장할 데이터가 없습니다.";
                    return;
                }

                // 변경된 엔티티 개수를 카운트합니다.
                int updateCount = 0;

                // 각 엔티티에 대해 DB에 있는 기존 데이터와 비교
                foreach (var currentEntity in EsolutionPopulations) 
                {
                    // 필수 값 확인 (예: EmployeeNumber가 0이면 무시)
                    if (currentEntity.EmployeeNumber == 0)
                        continue;

                    // DB에서 기존 데이터를 가져옴 (동일 EmployeeNumber 기준)

                    var dbEntity = await Task.Run(() => databaseService.GetDetail(currentEntity.EmployeeNumber));

                    if (dbEntity == null)
                    {
                        // 기존 데이터가 없는 경우 신규 생성 처리
                        await Task.Run(() => databaseService.Create(currentEntity)).ConfigureAwait(false);
                        updateCount++;
                    }
                    else
                    {
                        // 두 객체의 중요한 필드를 비교하여 차이가 있으면 업데이트
                        if (!AreEntitiesEqual(dbEntity, currentEntity))
                        {
                            // 필요한 부분만 매핑하거나 전체 객체를 업데이트 (비교/매핑 로직은 상황에 따라 추가)
                            await Task.Run(() => databaseService.Update(currentEntity)).ConfigureAwait(false);
                            updateCount++;
                        }
                    }
                }
                StatusMessage = updateCount > 0
                    ? $"{updateCount}건의 변경된 데이터가 저장되었습니다."
                    : "변경된 내용이 없습니다.";


                //// (원하는 경우) 저장 후 최신 DB 데이터를 다시 UI에 반영
                this.EmployeeNumber = this.EsolutionPopulations.Select(x => x.EmployeeNumber.ToString());
                await RefreshDataAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"저장 중 오류 발생: {ex.Message}";
            }
        }

        private readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

        private bool AreEntitiesEqual(object dbEntity, object currentEntity)
        {
            if (dbEntity == null || currentEntity == null) // 클래스가 null인 경우 false 반환
                return false;

            // 두 객체의 타입이 동일한지 확인
            var type = dbEntity.GetType(); //Type은 클래스의 메타데이터를 가지고 있음
            if (dbEntity.GetType() != currentEntity.GetType()) 
                return false;

            // 성능 최적화를 위해 PropertyInfo를 캐싱
            var properties = PropertyCache.GetOrAdd(type, t => t.GetProperties());

            foreach (var prop in properties)
            {
             
                var dbValue = prop.GetValue(dbEntity);
                var currentValue = prop.GetValue(currentEntity);

                if (!Equals(dbValue, currentValue))
                    return false;
            }

            return true;
        }
        




        #endregion

    }
}

//[RelayCommand] 
//private void SearchData()
//{
//    if (string.IsNullOrWhiteSpace(SearchCriteria))
//    {
//        StatusMessage = "검색 조건을 입력하세요.";
//        return;
//    }

//    try
//    {
//        FilteredResults = salaryService.Search(entity =>
//            entity.Name.Contains(SearchCriteria, StringComparison.OrdinalIgnoreCase));

//        StatusMessage = $"{FilteredResults?.Count() ?? 0}개의 결과를 찾았습니다.";
//    }
//    catch (Exception ex)
//    {
//        StatusMessage = $"검색 중 오류 발생: {ex.Message}";
//    }
//}


///// <summary>
///// 데이터를 필터링하는 커맨드
///// </summary>
//[RelayCommand]
//private void FilterData()
//{
//    try
//    {
//        FilteredResults = salaryService.Filter(entity => entity.Basepay >= 5000);

//        StatusMessage = $"{FilteredResults?.Count() ?? 0}개의 데이터가 필터링되었습니다.";
//    }
//    catch (Exception ex)
//    {
//        StatusMessage = $"필터링 중 오류 발생: {ex.Message}";
//    }
//}


//private void ClearSelectedData()
//{
//    SelectedEmployeeNumber = null;
//    SelectedName = null;
//    SelectedDepartment = null;
//    SelectedPosition = null;
//    SelectedDateS = null;
//    SelectedDateE = null;
//    SelectedBasePay = null;
//    SelectedRDExpenses = null;
//    SelectedOvertimePay = null;
//    SelectedPerformancePay = null;
//    SelectedMealExpenses = null;
//    SelectedSelfDrivingSubsidy = null;
//    SelectedIncomeTax = null;
//    SelectedLongTermCareInsurance = null;
//    SelectedLocalIncomeTax = null;
//    SelectedEmploymentInsurance = null;
//    SelectedNationalPensionPlan = null;
//    SelectedEmploymentInsuranceYearEndSettlement = null;
//    SelectedHealthInsurance = null;
//    SelectedNationalPensionRetroactive = null;
//    SelectedTotalPayment = null;
//    SelectedTotalDeductions = null;
//    SelectedLoanPayment = null;
//    SelectedLastPaymentDate = null;

//}

//[RelayCommand]
//private void CreateNewData()
//{

//}




// ────── 동적 컬럼 생성 커맨드 ──────

/// <summary>
/// 좌측 클릭 시 (그룹 //1) 동적 컬럼 생성
/// EmploymentInsurance 컬럼 기준, 5% 증가한 값을 예제로 사용
/// </summary>
// 동적 컬럼 생성 함수

// 3. 동적 컬럼 정의를 로드 및 저장 

//[RelayCommand]
//private void CreateDynamicColumnRight()
//{
//    string inputName = Interaction.InputBox("우측 동적 컬럼 이름을 입력하세요.");
//    if (string.IsNullOrWhiteSpace(inputName))
//    {
//        StatusMessage = "동적 컬럼 이름이 입력되지 않았습니다.";
//        System.Windows.MessageBox.Show(StatusMessage);
//        return;
//    }

//    string columnKey = $"DynamicR_{inputName}";

//    // 동적 컬럼 키를 맨 끝에 추가
//    if (!DynamicColumnKeys.Contains(columnKey))
//    {
//        DynamicColumnKeys.Add(columnKey);

//        // 모든 데이터에 새로운 속성 추가
//        foreach (var item in EsolutionPopulations)
//        {
//            if (!item.DynamicProperties.ContainsKey(columnKey))
//                item.DynamicProperties[columnKey] = null; // 초기값 설정
//        }

//        // UI 갱신
//        OnPropertyChanged(nameof(EsolutionPopulations));
//    }
//}

//private void LoadDynamicColumnDefinitions()
//{
//    if (File.Exists(settingsFilePath))
//    {
//        var json = File.ReadAllText(settingsFilePath);
//        var definitions = JsonSerializer.Deserialize<List<DynamicColumnDefinition>>(json);

//        if (definitions != null)
//        {
//            foreach (var def in definitions)
//            {
//                DynamicColumnDefinitions.Add(def);
//            }
//        }
//        else
//        {
//            // definitions가 null인 경우 처리
//            // 예를 들어, 오류를 기록하거나 기본값으로 초기화
//        }
//    }
//}


//private void SaveDynamicColumnDefinitions()
//{
//    var definitions = dynamicColumnKeys.Select(key => new DynamicColumnDefinition(key, key.Replace("DynamicL_", ""), dynamicColumnKeys.IndexOf(key), "DynamicL_")).ToList();
//    var json = JsonSerializer.Serialize(definitions, new JsonSerializerOptions { WriteIndented = true });
//    File.WriteAllText(settingsFilePath, json);

//    // 로그 추가
//    Console.WriteLine("동적 열 정의 저장됨");
//    Console.WriteLine(json);
//}


//[RelayCommand]
//private void CreateDynamicColumnLeft()
//{
//    string inputName = Interaction.InputBox("좌측 동적 컬럼 이름을 입력하세요.");
//    if (string.IsNullOrWhiteSpace(inputName))
//    {
//        System.Windows.MessageBox.Show("동적 컬럼 이름이 입력되지 않았습니다.");
//        return;
//    }

//    string columnKey = $"DynamicL_{inputName}";

//    // 동적 컬럼 키를 맨 앞에 추가
//    if (!dynamicColumnKeys.Contains(columnKey))
//    {
//        dynamicColumnKeys.Insert(0, columnKey);

//        foreach (var item in EsolutionPopulations)
//        {
//            if (!item.DynamicProperties.ContainsKey(columnKey))
//                item.DynamicProperties[columnKey] = null; // 초기값 설정
//        }

//        // 동적 열 키가 추가됨을 알림
//        OnPropertyChanged(nameof(DynamicColumnKeys));

//        // 로그 추가
//        Console.WriteLine($"동적 열 키 추가됨: {columnKey}");

//        // 데이터 변경 알림
//        OnPropertyChanged(nameof(EsolutionPopulations));

//        // 동적 열 정의 저장
//        SaveDynamicColumnDefinitions();
//    }
//}

//[RelayCommand]
//private void CreateDynamicColumnRight()
//{
//    string inputName = Interaction.InputBox("우측 동적 컬럼 이름을 입력하세요.");
//    if (string.IsNullOrWhiteSpace(inputName))
//    {
//        System.Windows.MessageBox.Show("동적 컬럼 이름이 입력되지 않았습니다.");
//        return;
//    }

//    string columnKey = $"DynamicR_{inputName}";

//    // 동적 컬럼 키를 맨 끝에 추가
//    if (!dynamicColumnKeys.Contains(columnKey))
//    {
//        dynamicColumnKeys.Add(columnKey);

//        foreach (var item in EsolutionPopulations)
//        {
//            if (!item.DynamicProperties.ContainsKey(columnKey))
//                item.DynamicProperties[columnKey] = null; // 초기값 설정
//        }

//        OnPropertyChanged(nameof(EsolutionPopulations));
//    }
//}

//private void AddDynamicColumns(Wpf.Ui.Controls.DataGrid dataGrid)
//{
//    // 기존 동적 열 제거 (중복 방지)
//    var existingDynamicColumns = dataGrid.Columns.Where(c => c.Header.ToString().StartsWith("Dynamic")).ToList();
//    foreach (var column in existingDynamicColumns)
//    {
//        dataGrid.Columns.Remove(column);
//    }

//    // DynamicColumnKeys 순서대로 열 추가
//    foreach (var columnKey in DynamicColumnKeys)
//    {
//        if (!dataGrid.Columns.Any(c => c.Header.ToString() == columnKey))
//        {
//            var dynamicColumn = new DataGridTextColumn
//            {
//                Header = columnKey,
//                Binding = new Binding($"DynamicProperties[{columnKey}]")
//            };
//            dataGrid.Columns.Add(dynamicColumn);
//        }
//    }
//}

