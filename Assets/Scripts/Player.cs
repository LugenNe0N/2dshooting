using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    public int m_levelMax; // レベルの最大値
    public int m_hpMax; // HP の最大値
    public float m_magnetDistanceFrom; // 宝石を引きつける距離（レベルが最小値の時）
    public float m_magnetDistanceTo; // 宝石を引きつける距離（レベルが最大値の時）
    public int m_nextExpBase; // 次のレベルまでに必要な経験値の基本値
    public int m_nextExpInterval; // 次のレベルまでに必要な経験値の増加値

    public int m_level; // レベル
    public int m_exp; // 経験値
    public int m_hp; // HP
    public float m_magnetDistance; // 宝石を引きつける距離
    public int m_prevNeedExp; // 前のレベルに必要だった経験値
    public int m_needExp; // 次のレベルに必要な経験値

    public static Player m_instance; // プレイヤーのインスタンスを管理する static 変数

    public int m_damage; // この敵がプレイヤーに与えるダメージ

    // ゲーム開始時に呼び出される関数
    private void Awake()
    {
        // 他のクラスからプレイヤーを参照できるように
        // static 変数にインスタンス情報を格納する
        m_instance = this;

        // 各種パラメータを初期化する
        m_level = 1; // レベル
        m_hp = m_hpMax; // HP
        m_magnetDistance = m_magnetDistanceFrom; // 宝石を引きつける距離
        m_needExp = GetNeedExp(1); // 次のレベルに必要な経験値
    }

    // Spaceshipコンポーネント
    Spaceship spaceship;

    IEnumerator Start()
    {
        // Spaceshipコンポーネントを取得
        spaceship = GetComponent<Spaceship>();

        while (true)
        {

            // 弾をプレイヤーと同じ位置/角度で作成
            spaceship.Shot(transform);

            // ショット音を鳴らす
            GetComponent<AudioSource>().Play();

            // shotDelay秒待つ
            yield return new WaitForSeconds(spaceship.shotDelay);
        }
    }

    void Update()
    {
        // 右・左
        float x = CrossPlatformInputManager.GetAxisRaw("Horizontal")/2;

        // 上・下
        float y = CrossPlatformInputManager.GetAxisRaw("Vertical")/2;

        // 移動する向きを求める
        Vector2 direction = new Vector2(x, y).normalized;

        // 移動の制限
        Move(direction);
    }

    // 機体の移動
    void Move(Vector2 direction)
    {
        // 画面左下のワールド座標をビューポートから取得
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        // 画面右上のワールド座標をビューポートから取得
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // プレイヤーの座標を取得
        Vector2 pos = transform.position;

        // 移動量を加える
        pos += direction * spaceship.speed * Time.deltaTime;

        // プレイヤーの位置が画面内に収まるように制限をかける
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        // 制限をかけた値をプレイヤーの位置とする
        transform.position = pos;
    }

    // ぶつかった瞬間に呼び出される
    void OnTriggerEnter2D(Collider2D c)
    {
        m_damage = 100;
        // レイヤー名を取得
        string layerName = LayerMask.LayerToName(c.gameObject.layer);

        // レイヤー名がBullet (Enemy)の時は弾を削除
        if (layerName == "Bullet (Enemy)")
        {
            // 弾の削除
            Destroy(c.gameObject);
        }

        // レイヤー名がBullet (Enemy)またはEnemyの場合は爆発
        if (layerName == "Bullet (Enemy)" || layerName == "Enemy")
        {
            //// Managerコンポーネントをシーン内から探して取得し、GameOverメソッドを呼び出す
            //FindObjectOfType<Manager>().GameOver();

            //// 爆発する
            //spaceship.Explosion();

            //// プレイヤーを削除
            //Destroy(gameObject);
            Damage(m_damage);
            
        }
    }

    // 経験値を増やす関数
    // 宝石を取得した時に呼び出される
    public void AddExp(int exp)
    {
        // 経験値を増やす
        m_exp += exp;

        // まだレベルアップに必要な経験値に足りていない場合、ここで処理を終える
        if (m_exp < m_needExp) return;

        // レベルアップする
        m_level++;

        // 今回のレベルアップに必要だった経験値を記憶しておく
        // （経験値ゲージの表示に使用するため）
        m_prevNeedExp = m_needExp;

        // 次のレベルアップに必要な経験値を計算する
        m_needExp = GetNeedExp(m_level);

        // レベルアップしたので、各種パラメータを更新する
        var t = (float)(m_level - 1) / (m_levelMax - 1);
        m_magnetDistance = Mathf.Lerp(m_magnetDistanceFrom, m_magnetDistanceTo, t); // 宝石を引きつける距離
        m_hp = m_hpMax;
    }

    // 指定されたレベルに必要な経験値を計算する関数
    private int GetNeedExp(int level)
    {
        /*
         * 例えば、m_nextExpBase が 16、m_nextExpInterval が 18 の場合、
         *
         * レベル 1：16 + 18 * 0 = 16
         * レベル 2：16 + 18 * 1 = 34
         * レベル 3：16 + 18 * 4 = 88
         * レベル 4：16 + 18 * 9 = 178
         *
         * このような計算式になり、レベルが上がるほど必要な経験値が増えていく
         */
        return m_nextExpBase +
            m_nextExpInterval * ((level - 1) * (level - 1));
    }

    // ダメージを受ける関数
    // 敵とぶつかった時に呼び出される
    public void Damage(int damage)
    {
        // HP を減らす
        m_hp -= damage;

        // HP がまだある場合、ここで処理を終える
        if (0 < m_hp) return;

        // プレイヤーが死亡したので非表示にする
        // 本来であれば、ここでゲームオーバー演出を再生したりする
        // Managerコンポーネントをシーン内から探して取得し、GameOverメソッドを呼び出す
        FindObjectOfType<Manager>().GameOver();

        // 爆発する
        spaceship.Explosion();

        // プレイヤーを削除
        Destroy(gameObject);
    }
}