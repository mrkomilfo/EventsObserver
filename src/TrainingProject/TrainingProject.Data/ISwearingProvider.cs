using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TrainingProject.Data
{
    public interface ISwearingProvider
    {
        Task<IList<string>> GetSwearingAsync();
    }
}
