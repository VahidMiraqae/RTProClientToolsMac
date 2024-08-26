namespace ClientToolsMac.MacPrint;

public enum PrintSides
{
    OneSide = 0, // default
    TwoSidePortrait, // -o sides=two-sided-long-edge
    TwoSideLandscape, // -o sides=two-sided-short-edge
}


// lpr -l filename