using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using UiDesktopApp2.Interfaces;
using UiDesktopApp2.Models;

namespace UiDesktopApp2.Services
{    // Interface IDATABASE, ISALARY, IDATETIME을 상속받음 T는 EsolutionPopulation
    class EsolutionPopulationService : IDatabase<EsolutionPopulation>,ISalary<EsolutionPopulation>, IDateTime
    {
        private readonly EsolutionDatabaseContext? esolutionDatabaseContext; //수정 불가능한 데이터베이스 컨텍스트
        private readonly Subject<EsolutionPopulation> _updates = new Subject<EsolutionPopulation>(); // Subject은 Observable과 Observer를 상속받음 Observable은 데이터를 발행하고 Observer는 데이터를 구독함

        public DateTime? GetCurrentTime()
        {
            return DateTime.Now;
        }

        public EsolutionPopulationService(EsolutionDatabaseContext context) //의존성 주입
        {
            esolutionDatabaseContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        private EsolutionPopulation GetEntityById(int? id)
        {
            var entity = esolutionDatabaseContext?.EsolutionPopulations.Find(id); //db에서 id로 entity 찾고 행 반환
            if (entity == null)
                throw new KeyNotFoundException("Entity not found.");
            return entity;
        }

        public void Create(EsolutionPopulation entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                esolutionDatabaseContext?.Add(entity);
                esolutionDatabaseContext?.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception using a logging framework
                Console.WriteLine($"Error: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public void Delete(int? id)
        {
            try
            {
                var entity = GetEntityById(id); //
                esolutionDatabaseContext?.Remove(entity); //db에서 entity 삭제
                esolutionDatabaseContext?.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new ApplicationException("An error occurred while deleting the entity.", ex);
            }
        }

        public List<EsolutionPopulation>? Get()
        {
            return esolutionDatabaseContext?.EsolutionPopulations.ToList();
        }

        public EsolutionPopulation? GetDetail(int? name)
        {
            return esolutionDatabaseContext?.EsolutionPopulations.Find(name);
        }

        public void NotifyUpdate(EsolutionPopulation update)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));
            _updates.OnNext(update); // Subject를 통해 알림 전송
        }

        public IObservable<EsolutionPopulation> SubscribeToUpdates()
        {
            return _updates.AsObservable(); // Subject를 Observable로 노출
        }

        public List<EsolutionPopulation>? Search(Func<EsolutionPopulation, bool> criteria)
        {
            return esolutionDatabaseContext?.EsolutionPopulations.AsQueryable().Where(criteria).ToList();
        }

        public  void Update(EsolutionPopulation entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingEntity = esolutionDatabaseContext?.EsolutionPopulations.Find(entity.EmployeeNumber);
            if (existingEntity == null)
                throw new KeyNotFoundException("Entity not found.");

            existingEntity.Name = entity.Name;
            existingEntity.EmployeeNumber = entity.EmployeeNumber;

            esolutionDatabaseContext?.Update(existingEntity);
            esolutionDatabaseContext?.SaveChanges();
        }

        public List<EsolutionPopulation>? Filter(Func<EsolutionPopulation, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExportWindowToPdf(Window window, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}


