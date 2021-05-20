/// Reprsents the parent for all data classes.
/// Data classes hold logical information for game objects.
public class DObject {
  private static ushort ID_CURSOR = 0;

  public ushort ID { get; private set; }

  public DObject(ushort id) {
    ID = id;
  }

  public DObject() {
    ID = ID_CURSOR;
    ID_CURSOR++;
  }
}