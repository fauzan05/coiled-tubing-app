using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coiled_tubing_app.Services
{
    public class RecordService
    {
        private readonly FileHistoryService _fileHistoryService;

        public RecordService()
        {
            _fileHistoryService = new FileHistoryService();
        }

        public async Task<()>
    }
}
