using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Api.Controllers;

namespace Web_Api.Interfaces {
  public interface INearByFinder 
  {
    PositionResponseDTO[] GetNearby(Guid id);
  }
}
