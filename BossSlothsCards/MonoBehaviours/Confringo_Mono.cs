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
        private void Awake()
        {
            GameModeManager.AddHook(GameModeHooks.HookPointStart, (gm) => WaitFor5SecondsAndDoSomething());
        }

        private IEnumerator WaitFor5SecondsAndDoSomething()
        {
            if (this == null) yield break;
            var foundPlayerWithCard = false;
            foreach (var player in PlayerManager.instance.players)
            {
                if (player.GetComponent<Confringo_Mono>())
                {
                    foundPlayerWithCard = true;
                }
            }
            if (!foundPlayerWithCard) yield break;
            yield return new WaitForSeconds(5);
            
            if ( GetComponent<PhotonView>().IsMine)
            {
                var rng = new System.Random();
                var seed = rng.Next(0, 5000);
                GetComponent<PhotonView>().RPC("RPCA_ExplodeBlock", RpcTarget.All, seed);
            }
        }

        private static bool Condition(GameObject obj)
        {
            return obj.GetComponent<SpriteRenderer>() && !obj.name.Contains("Color") && !obj.name.Contains("Lines");
        }

        [PunRPC]
        public void RPCA_ExplodeBlock(int seed)
        {
            var scene = SceneManager.GetSceneAt(1);
            if (!scene.IsValid())  return;
            var objects = scene.GetRootGameObjects()[0].GetComponentsInChildren<SpriteRenderer>(false);
            if (objects == null)  return;

            GameObject randomObject;
            var loops = 0;
            while (true)
            {
                BACK:
                var rng = new System.Random(seed);
                var rID = rng.Next(0, objects.Length);
                if (objects[rID] == null) continue;
                randomObject = objects[rID].gameObject;
                if (Condition(randomObject))
                {
                    break;
                    //#TODO why does it not work on the other player and it keeps looping
                }
                else
                {
                    loops++;
                    if (loops >= 100)
                    {
                        UnityEngine.Debug.LogError("Couldn't find object in 100 iterations");
                        return;
                    }
                    seed++;
                    goto  BACK;
                }
            }
            
            var pieces = BossSlothCards.EffectAsset.LoadAsset<GameObject>("Pieces");
            var _pieces = Instantiate(pieces, randomObject.transform.parent);
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
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, (gm) => WaitFor5SecondsAndDoSomething());
        }
    }
}