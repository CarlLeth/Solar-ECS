using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Globalization.Translations
{
    public interface ITextCulture
    {
        bool IsAvailable { get; }
        Guid Id { get; }
    }
}
