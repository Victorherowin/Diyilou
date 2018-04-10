# First Floor Guide
------
## 编写一个普通技能
### 1.继承SkillBase,并重载OnReset，OnUse，OnHit
```csharp
    using Skill;
    
    class ExampleSkill:SkillBase
    {
        private static string[] s_animiation_names = new string[] { "skill01" };//动画状态机中对应技能的动画名

        public MonsterAttAttack()
        {
            AnimationNames = s_animiation_names;
        }

        public override void OnHit(EntityBase self, EntityBase target)//击中目标时调用
        {
            ApplyFormula(self,target);
        }

        public override void OnReset(Animator ani)//重置动画状态机参数
        {
            ani.ResetTrigger("Skill1");
        }

        public override void OnUse(EntityBase self, Animator ani)//设置动画状态机参数
        {
            ani.SetTrigger("Skill1");
        }
    }
```
### 2.注册技能
在继承了EntityBase类的类中的Start中写上
```csharp
    SkillManager.RegisterSkill<ExampleSkill>();
```
如果是PlayerEntity类则使用BindSkill方法
```csharp
    PlayerSkillManager.Instance.BindSkill<ExampleSkill>("Touch");
```
## 编写一个蓄力技能
### 1.继承SkillBase和实现ISkillHoldable，按照需求实现OnHoldEnter，OnHoldExit,OnHold和重载OnReset，OnUse，OnHit
```csharp
    class ExampleSkill:SkillBase,ISkillHoldable
```
### 2.注册技能
在继承了EntityBase类的类中的Start中写上
```csharp
    SkillManager.RegisterHoldSkill<ExampleSkill>();
```
如果是PlayerEntity类则使用BindHoldSkill方法
```csharp
    PlayerSkillManager.Instance.BindHoldSkill<ExampleSkill>("Touch");
``` 
## 编写一个远程技能
### 1.继承SkillBase和实现ISkillLongDistanceable，实现OnBulletHit和重载OnReset，OnUse，OnHit
```csharp
    class ExampleSkill:SkillBase,ISkillLongDistanceable
```

#### OnUse在中使用entity.Shot方法发射子弹
```csharp
//public static void Shot(this EntityBase shooter, Vector3 shoot_position, 
//						   GameObject bullet_prefab,ISkillLongDistanceable skill, Vector3 velocity);
public override void OnUse(EntityBase self, Animator ani)
{
	Vector3 pos = self.transform.position;
   pos.y += 3.8f;
   Vector3 dir = self.transform.Find("ShotLine/Sphere").position - pos;

   dir.Normalize();
   self.Shot(pos + dir, Resources.Load("Prefabs/Bullet/Test/TestBullet") as GameObject, this, dir * m_force);
}
```

### 2.注册技能
在继承了EntityBase类的类中的Start中写上
```csharp
    SkillManager.RegisterSkill<ExampleSkill>();
```
如果是PlayerEntity类则使用BindHoldSkill方法
```csharp
    PlayerSkillManager.Instance.BindSkill<ExampleSkill>("Remote",EntityStatus.Movement|EntityStatus.Grounded,StatusMatchOp.One,true, "ShotEnable");
```
## 编写一个组合技能
### 1.同 “编写一个普通技能”
### 2.注册技能
在继承了EntityBase类的类中的Start中写上
```csharp
    SkillManager.RegisterSkill<ExampleSkill>();
```
如果是PlayerEntity类则使用BindSkill方法
```csharp
    PlayerSkillManager.Instance.BindComboSkill<ExampleSkill>(new string[]{"Touch","Touch"，"Touch"，"Thump"});
```

## 编写一个远程蓄力技能
### 1.继承SkillBase和实现ISkillHoldable，ISkillLongDistanceable
### 2.同“编写一个蓄力技能”

------
## 获取当前正在使用的技能
```cshape
	ISkill skill=entity.SkillManager.GetActivateSkill();
```
## 获取当前正在使用的技能的上下文
```cshape
	SkillContext skill=entity.SkillManager.GetActivateSkillContext();
```