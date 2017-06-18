using System.Collections;

public interface IGenPipeline
{
    IEnumerator Execute(DungeonMap map);
    void OnDrawGizmosSelected(DungeonMap map);
}