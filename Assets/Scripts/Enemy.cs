using UnityEngine;
using System.Collections;
using System.Linq;

public class Enemy : MonoBehaviour
{
	// ヒットポイント
	public int hp = 1;

	// スコアのポイント
	public int point = 100;

    public Gem[] m_gemPrefabs; // 宝石のプレハブを管理する配列
    public float m_gemSpeedMin; // 生成する宝石の移動の速さ（最小値）
    public float m_gemSpeedMax; // 生成する宝石の移動の速さ（最大値）
    public int m_exp; // この敵を倒した時に獲得できる経験値
    public int m_damage; // この敵がプレイヤーに与えるダメージ

    // Spaceshipコンポーネント
    Spaceship spaceship;

	IEnumerator Start ()
	{
		
		// Spaceshipコンポーネントを取得
		spaceship = GetComponent<Spaceship> ();
		
		// ローカル座標のY軸のマイナス方向に移動する
		Move (transform.up * -1);
		
		// canShotがfalseの場合、ここでコルーチンを終了させる
		if (spaceship.canShot == false) {
			yield break;
		}
			
		while (true) {
			
			// 子要素を全て取得する
			for (int i = 0; i < transform.childCount; i++) {
				
				Transform shotPosition = transform.GetChild (i);
				
				// ShotPositionの位置/角度で弾を撃つ
				spaceship.Shot (shotPosition);
			}
			
			// shotDelay秒待つ
			yield return new WaitForSeconds (spaceship.shotDelay);
		}
	}

	// 機体の移動
	public void Move (Vector2 direction)
	{
        GetComponent<Rigidbody2D>().velocity = direction * spaceship.speed;
    }

	void OnTriggerEnter2D (Collider2D c)
	{
		// レイヤー名を取得
		string layerName = LayerMask.LayerToName (c.gameObject.layer);
		
		// レイヤー名がBullet (Player)以外の時は何も行わない
		if (layerName != "Bullet (Player)") return;

		// PlayerBulletのTransformを取得
		Transform playerBulletTransform = c.transform.parent;

		// Bulletコンポーネントを取得
		Bullet bullet =  playerBulletTransform.GetComponent<Bullet>();

		// ヒットポイントを減らす
		hp = hp - bullet.power;

		// 弾の削除
		Destroy(c.gameObject);

		// ヒットポイントが0以下であれば
		if(hp <= 0 )
		{
            // エネミーの現在地を取得する
            var EnemyPos = this.transform.position;

            // スコアコンポーネントを取得してポイントを追加
            FindObjectOfType<Score>().AddPoint(point);

			// 爆発
			spaceship.Explosion ();
			
			// エネミーの削除
			Destroy (gameObject);

            var exp = m_exp;

            while (0 < exp)
            {
                // 生成可能な宝石を配列で取得する
                var gemPrefabs = m_gemPrefabs.Where(col => col.m_exp <= exp).ToArray();

                // 生成可能な宝石の配列から、生成する宝石をランダムに決定する
                var gemPrefab = gemPrefabs[Random.Range(0, gemPrefabs.Length)];

                // 敵の位置に宝石を生成する
                var gem = Instantiate(
                    gemPrefab, EnemyPos, Quaternion.identity);

                // 宝石を初期化する
                gem.Init(m_exp, m_gemSpeedMin, m_gemSpeedMax);

                // まだ宝石を生成できるかどうか計算する
                exp -= gem.m_exp;
            }

        }
        else{
			// Damageトリガーをセット
			spaceship.GetAnimator().SetTrigger("Damage");
		
		}
	}
}