using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB{
    public partial class LiteDatabase{
        public event EventHandler OnDisposing;

        protected virtual void OnDispose(EventArgs EventArgs) => OnDisposing?.Invoke(this, EventArgs);
        }
    }
