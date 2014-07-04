using SettingsHandlerInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WeatherInfo.Classes
{
    class LoadSettingsHandler
    {
        /// <summary>
        /// Возвращает список (словарь) доступных в указанной директории обработчиков.
        /// </summary>
        /// <returns>Список (словарь) обработчиков.</returns>
        private static Dictionary<int, string> GetLibrariesInDirectory(string dirPath)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dirPath))
                {
                    throw new ApplicationException("Путь к рабочей папке не найден!");
                }

                var libs = new Dictionary<int, string>();

                var pathesToLibs = Directory.GetFiles(dirPath, "*SettingsHandler.dll");
                foreach (var path in pathesToLibs)
                {
                    libs.Add(libs.Count + 1, path);
                }

                return libs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Возвращает имя (путь) выбранного пользователем обработчика.
        /// </summary>
        /// <param name="libs">Список (словарь) доступных обработчиков.</param>
        /// <returns>Имя (путь) выбранного пользователем обработчика</returns>
        private static string SelectHandler(Dictionary<int, string> libs)
        {
            try
            {
                //string chooseLibName;
                //while (true)
                //{
                //    PrintListOfLibs(libs);

                //    var key = Console.ReadLine();
                //    if (!String.IsNullOrWhiteSpace(key))
                //    {
                //        if (libs.TryGetValue(Int32.Parse(key), out chooseLibName))
                //        {
                //            break;
                //        }
                //    }
                //}
                //return chooseLibName;

                return "XMLSettingsHandler.dll";//"MdfDbSettingsHandler.dll";
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Загружает выбранный обработчик и передает ему список имен входных и выходных файлов для обработки.
        /// </summary>
        /// <param name="handlerLibName">Имя обработчика.</param>
        /// <param name="dictFileName">Имя (путь) файла словаря (без указания расширения файла).</param>
        /// <param name="textFileName">Имя (путь) входного файла с текстом (без указания расширения файла).</param>
        /// <param name="resultFileName">Имя (путь) результирующего файла (без указания расширения файла).</param>
        private static ISettingsHandler LoadHandler(string handlerLibName)
        {
            try
            {
                if (!File.Exists(handlerLibName))
                {
                    throw new ApplicationException("Библиотека обработчика " + handlerLibName + " не найдена!");
                }

                var handlerAssembly = Assembly.LoadFrom(handlerLibName);
                foreach (var t in handlerAssembly.GetExportedTypes())
                {
                    if (t.IsClass && typeof(ISettingsHandler).IsAssignableFrom(t))
                    {
                        ISettingsHandler handler = (ISettingsHandler)Activator.CreateInstance(t);
                        return handler;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    
        public static ISettingsHandler GetInstanceSettingsHandler() {
            try
            {
                var libs = GetLibrariesInDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                if (libs == null || libs.Count == 0)
                {
                    throw new ApplicationException("Не удалось найти ни одного обработчика!");
                }

                var chooseLibName = SelectHandler(libs);
                if (String.IsNullOrWhiteSpace(chooseLibName))
                {
                    throw new ApplicationException("Несуществующий путь к обработчику!");
                }

                ISettingsHandler handler = LoadHandler(chooseLibName);

                return handler;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
