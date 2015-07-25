namespace DXGame.Core.Behavior
{
    /*
        Triggers serve as the gateways to states. Triggers should be of the form (has some condition been fulfilled?)
    */

    public delegate bool Trigger();
}