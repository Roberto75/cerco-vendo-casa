using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Models
{
    public class Pagedlmmobili : Shared.Paged
    {
              public IEnumerable<Immobile> Immobili { get; set; }

        
    }
}
