using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// An IStrategy is series of imageshifts at different tilts.
///
public interface IStrategy
{
    // Request a list of exposures (image shifts at tilts)
    List<Exposure> GetExposures();
}
