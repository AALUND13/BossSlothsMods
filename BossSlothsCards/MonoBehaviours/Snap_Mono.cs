using System.Collections;
using Photon.Pun;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BossSlothsCards.MonoBehaviours
{
    public class Snap_Mono : BossSlothMonoBehaviour
    {
        private Player player;
        private bool hasPointHookBeenMade;
        private void Awake()
        {
            if (!hasPointHookBeenMade)
            {
                GameModeManager.AddHook(GameModeHooks.HookPointStart, (gm) => DoExplosionThings());
                GameModeManager.AddHook(GameModeHooks.HookPointEnd, (gm) => IeStopAllCoroutines());
                hasPointHookBeenMade = true;
            }
            player = GetComponent<Player>();
        }

        private IEnumerator IeStopAllCoroutines()
        {
            StopThings();
            yield break;
        }

        private void StopThings()
        {
            StopAllCoroutines();
        }

        private IEnumerator DoExplosionThings()
        {
            StopCoroutine(DoThings());
            if (this == null) yield break;
            //Wait5SecondsAndDoSomething();
            StartCoroutine(DoThings());
        }

        private IEnumerator DoThings()
        {
            if (!player.GetComponent<Snap_Mono>()) yield break;
            yield return new WaitForSeconds(7);
            var scene = SceneManager.GetSceneAt(1);
            if (!scene.IsValid())  yield break;
            var objects = scene.GetRootGameObjects()[0].GetComponentsInChildren<SpriteRenderer>(false);
            if (objects == null)  yield break;

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

                    StartCoroutine(DoThings());
                    yield break;
                }
            
                loops++;
                if (loops >= 500)
                {
                    UnityEngine.Debug.LogError("Couldn't find object in 500 iterations");
                    yield break;
                }
            }
        }

        private static bool Condition(GameObject obj)
        {
            return obj.activeInHierarchy && obj.GetComponent<SpriteRenderer>() && obj.name != "Color" && obj.name != "Lines";
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
            hasPointHookBeenMade = false;
        }
    }
}