using UnityEngine;

namespace Assets.Scripts {
  public class Networking : MonoBehaviour {
    static ILogger logger = Debug.logger;

    void Start() {
      logger.filterLogType = LogType.Log;
    }
  }
}