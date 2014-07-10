namespace DataHandlerInterface.Classes
{
    /// <summary>
    /// Класс региона города
    /// </summary>
    public class RegionOfCity
    {
        public int regionId { get; set; }
        public string regionName { get; set; }

        public RegionOfCity() { }

        /// <summary>
        /// Конструктор класса, создаёт экземпляр, хранящий информацию о регионе города
        /// </summary>
        /// <param name="_regionId">ИД региона</param>
        /// <param name="_regionName">Наименование региона</param>
        public RegionOfCity(int _regionId, string _regionName)
        {
            regionId = _regionId;
            regionName = _regionName;
        }
    }
}
