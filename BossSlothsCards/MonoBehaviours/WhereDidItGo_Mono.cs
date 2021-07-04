using System.Diagnostics;
using Photon.Pun;
using UnboundLib;
using UnityEngine;

namespace BossSlothsCards.MonoBehaviours
{
    public class WhereDidItGo_Mono : BossSlothMonoBehaviour
    {
        private static readonly System.Random rng = new System.Random();

        public void RemoveRandomObject(SpriteRenderer[] objects)
        {
            while (true)
            {
                return;
/*
                UnityEngine.Debug.LogWarning("looping");
                if (GameManager.instance.battleOngoing)
                {
                    //#TODO system type not found error with rng
                    while (true)
                    {
                        var rID = rng.Next(0, objects.Length);
                        if (objects[rID] == null) continue;
                        var randomObject = objects[rID].gameObject;
                        if (Condition(randomObject))
                        {
                            if (GetComponent<PhotonView>().IsMine)
                            {
                                GetComponent<PhotonView>().RPC("RPCA_ExplodeBlock", RpcTarget.All, new object[] {randomObject});
                            }
                        }
                        else
                        {
                            continue;
                        }

                        break;
                    }
                }
                else
                {
                    continue;
                }
                
                break;
*/
            }
        }

        private static bool Condition(GameObject obj)
        {
            return obj.GetComponent<SpriteRenderer>() && !obj.name.Contains("Color") && !obj.name.Contains("Lines");
        }

        [PunRPC]
        public void RPCA_ExplodeBlock(GameObject randomObject)
        {
            var pieces = BossSlothCards.EffectAsset.LoadAsset<GameObject>("Pieces");
            var _pieces = Instantiate(pieces, randomObject.transform.parent);
            _pieces.transform.position = randomObject.transform.position;
            _pieces.transform.rotation = randomObject.transform.rotation;
            randomObject.SetActive(false);
            this.ExecuteAfterSeconds(6, () =>
            {
                Destroy(_pieces);
            });
        }
    }
}