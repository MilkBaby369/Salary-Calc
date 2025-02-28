using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UiDesktopApp2.Interfaces;
using UiDesktopApp2.Models;
using Wpf.Ui.Controls;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
using UiDesktopApp2.Services;
using System.Windows.Threading;

namespace UiDesktopApp2.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        #region FIELD
        private bool isinitialized = false;


        private readonly IDatabase<EsolutionPopulation> databaseService;
        private readonly ISalary<EsolutionPopulation> emailService;

        private readonly IDateTime _dateTimeService;
        #endregion

        #region PROPERTIES
        // EsolutionPopulation 리스트
        [ObservableProperty]
        private IEnumerable<EsolutionPopulation>? esolutionPopulations;

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

        //  --------------------------------------------------------------

        [ObservableProperty]
        private int? selectedId;

        #endregion

        #region CONSTRUCTOR
        public DataViewModel(IDatabase<EsolutionPopulation> databaseService, ISalary<EsolutionPopulation> emailService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.esolutionPopulations = new ObservableCollection<EsolutionPopulation>();
            _dateTimeService = new DateTimeClass();

            ReadAllData();
        }
        #endregion
        public DateTime? CurrentTime => _dateTimeService.GetCurrentTime();

        #region COMMAND
        [RelayCommand]
        private void ReadDetailData() // Propery에 있는 SelectedId를 이용하여 데이터를 가져온다.
        {
            var targetData = this.databaseService?.GetDetail(this.SelectedEmployeeNumber);

            // targetData가 null이 아니면, 필드 값을 설정
            if (targetData != null)
            {
                this.SelectedEmployeeNumber = targetData.EmployeeNumber;
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
        }

        #endregion

        public async void OnNavigatedFrom()
        {
            if (!isinitialized)
            {
                await InitializeViewModelAsync();
                isinitialized = true;
            }
        }

        public void OnNavigatedTo()
        {
            //
        }

        private async Task InitializeViewModelAsync()
        {
            this.EsolutionPopulations = await Task.Run(() => this.databaseService?.Get());

            if (this.EsolutionPopulations != null)
            {
                this.EmployeeNumber = this.EsolutionPopulations.Select(x => x.EmployeeNumber.ToString());
            }


            isinitialized = true;


        }

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
                Console.WriteLine($"Error while reading data: {ex.Message}");
            }
        }


    }
}
