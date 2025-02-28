// An AmmoRefill that is instantiated as a part.
// The actual refill logic is handled by PlayerController.
public class AmmoRefillPart : Part
{
    // If trying to rotate an ammo, nothing happens
    public override void Rotate(float degrees)
    {
        return;
    }
}
