using System.Collections;
using Photon.Pun;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BossSlothsCards.MonoBehaviours
{
    public class Confringo_Mono : BossSlothMonoBehaviour
    {
        private Player player;
        private void Awake()
        {
            if (!BossSlothCards.hasPointHookBeenMade)
            {
                GameModeManager.AddHook(GameModeHooks.HookPointStart, (gm) => DoExplosionThings());
                BossSlothCards.hasPointHookBeenMade = true;
            }
            player = GetComponent<Player>();
        }

        private IEnumerator DoExplosionThings()
        {
            if (this == null) yield break;
            Wait5SecondsAndDoSomething();
        }

        private void Wait5SecondsAndDoSomething()
        {
            if (!player.GetComponent<Confringo_Mono>()) return;
            this.ExecuteAfterSeconds(5+player.playerID, () =>
            {
                var scene = SceneManager.GetSceneAt(1);
                if (!scene.IsValid())  return;
                var objects = scene.GetRootGameObjects()[0].GetComponentsInChildren<SpriteRenderer>(false);
                if (objects == null)  return;

                var loops = 0;
                while (true)
                {
                    var rng = new System.Random();
                    var rID = rng.Next(0, objects.Length);
                    if (objects[rID] == null) continue;
                    if (Condition(objects[rID].gameObject))
                    {
                        if (GetComponent<PhotonView>().IsMine)
                        {
                            GetComponent<PhotonView>().RPC("RPCA_ExplodeBlock", RpcTarget.All, rID);
                        }
                        break;
                    }
                
                    loops++;
                    if (loops >= 100)
                    {
                        UnityEngine.Debug.LogError("Couldn't find object in 100 iterations");
                        return;
                    }
                }
            });
        }

        private static bool Condition(GameObject obj)
        {
            return obj.activeInHierarchy &&obj.GetComponent<SpriteRenderer>() && !obj.name.Contains("Color") && !obj.name.Contains("Lines");
        }

        [PunRPC]
        public void RPCA_ExplodeBlock(int index)
        {
            var scene = SceneManager.GetSceneAt(1);
            if (!scene.IsValid())  return;
            var objects = scene.GetRootGameObjects()[0].GetComponentsInChildren<SpriteRenderer>(false);
            if (objects == null)  return;
            var randomObject = objects[index];

            var pieces = BossSlothCards.EffectAsset.LoadAsset<GameObject>("Pieces");
            var parent = randomObject.transform.parent;
            var _pieces = Instantiate(pieces, parent);
            var transform1 = randomObject.transform;
            _pieces.transform.position = transform1.position;
            _pieces.transform.rotation = transform1.rotation;
            randomObject.gameObject.SetActive(false);
            this.ExecuteAfterSeconds(6, () =>
            {
                Destroy(_pieces);
            });
        }

        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, (gm) => DoExplosionThings());
            BossSlothCards.hasPointHookBeenMade = false;
        }
    }
}