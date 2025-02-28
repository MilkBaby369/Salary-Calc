using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiDesktopApp2.Interfaces
{
    public interface ISalary<T>
    {
        /// <summary>
        /// 검색 조건에 따라 데이터를 검색합니다.
        /// </summary>
        /// <param name="criteria">검색 조건</param>
        /// <returns>조건에 맞는 데이터 리스트</returns>
        List<T>? Search(Func<T, bool> criteria);

        /// <summary>
        /// 특정 필드나 기준으로 데이터를 필터링합니다.
        /// </summary>
        /// <param name="filter">필터 조건</param>
        /// <returns>필터링된 데이터 리스트</returns>
        List<T>? Filter(Func<T, bool> filter);

        /// <summary>
        /// 실시간 데이터 변경을 구독합니다.
        /// </summary>
        /// <returns>데이터 변경 알림 스트림</returns>
        IObservable<T> SubscribeToUpdates();

        /// <summary>
        /// 데이터 변경 시 알림을 보냅니다.
        /// </summary>
        /// <param name="update">변경된 데이터</param>
        void NotifyUpdate(T update);

        /// <summary>
        /// WPF Window를 캡처하여 PDF로 저장합니다.
        /// </summary>
        /// <param name="window">캡처할 WPF Window</param>
        /// <param name="filePath">저장할 PDF 파일 경로</param>
        /// <returns>PDF 생성 성공 여부</returns>
        Task<bool> ExportWindowToPdf(Window window, string filePath);



    }
}

