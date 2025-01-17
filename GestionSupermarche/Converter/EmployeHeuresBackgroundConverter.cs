using System.Globalization;
using GestionSupermarche.Repositories;
using GestionSupermarche.Services;
using GestionSupermarche.Models;

namespace GestionSupermarche.Converters
{

    public class EmployeHeuresBackgroundConverter : IValueConverter
    {
        private readonly TempsTravailRepository _tempsTravailRepository;


        public EmployeHeuresBackgroundConverter()
        {
            _tempsTravailRepository = new TempsTravailRepository(Database.GetConnection());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Employe employe)
                return Colors.Transparent;

            try
            {
                var heuresAujourdhui = Task.Run(async () =>
                    await _tempsTravailRepository.GetHeuresAujourdhui(employe.IdEmploye)).Result;

                return heuresAujourdhui switch
                {
                    0 => Color.FromArgb("#FFCDD2"),
                    <= 8 => Color.FromArgb("#FFE0B2"),
                    _ => Color.FromArgb("#C8E6C9")
                };
            }
            catch (Exception)
            {
                return Colors.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}