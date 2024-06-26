using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerUpHandler
{
    // 상점 아이템
    [HideInInspector] public InventoryItem inventoryItem;

    // 상점 슬롯 숫자
    [SerializeField] private int inventorySlotNum;

    // 상점 아이템 이미지
    [SerializeField] private Image inventoryItemImage;

    // 플레이어 스크립트
    [SerializeField] private Player player;

    // 상점 아이템 구매 패널
    [SerializeField] private GameObject shopItemBuyPanel;

    // 상점 아이템 구매 이미지
    [SerializeField] private Image shopItemBuyImage;

    // 상점 아이템 구매 이름
    [SerializeField] private Text shopItemBuyNameText;

    // 상점 아이템 구매 스탯
    [SerializeField] private Text shopItemBuyStatusText;

    // 상점 아이템 가격
    [SerializeField] private Text shopItemBuyPriceText;

    public void UpdateSlotUI()
    {
        // 슬롯 UI를 업데이트하는 함수
        inventoryItemImage.sprite = inventoryItem.itemImage;
        inventoryItemImage.gameObject.SetActive(true);
    }

    public void RemoveSlot()
    {
        // 슬롯을 제거하는 함수
        inventoryItem = null;
        inventoryItemImage.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 상점 아이템 슬롯이 클릭되었을때
        // 상점 슬롯과 인벤토리 슬롯에서 아이템이 없는데 슬롯이 눌러지는 문제
        // 상점 아이템 리스트 갯수를 넘어서는 슬롯을 누르면 리턴
        if (player.shopDatabase.shopItemList.Count <= inventorySlotNum) return;

        // 상점 아이템 구매 패널을 보여준다
        shopItemBuyPanel.SetActive(true);

        // 상점 아이템 이미지를 보여준다
        shopItemBuyImage.sprite = player.shopSlots[inventorySlotNum].inventoryItem.itemImage;

        // 상점 아이템 이름을 보여준다
        shopItemBuyNameText.text = player.shopSlots[inventorySlotNum].inventoryItem.itemName;

        // 상점 아이템 가격을 보여준다
        shopItemBuyPriceText.text = player.shopSlots[inventorySlotNum].inventoryItem.price.ToString();

        // 상점 아이템 스탯을 보여준다
        // 장비
        if (player.shopSlots[inventorySlotNum].inventoryItem.itemType == ItemType.Equipment)
        {
            if (player.shopSlots[inventorySlotNum].inventoryItem.equipmentItemType == EquipmentItemType.Weapon) shopItemBuyStatusText.text = "공격력 + " + player.shopSlots[inventorySlotNum].inventoryItem.attack.ToString();
            else if (player.shopSlots[inventorySlotNum].inventoryItem.equipmentItemType == EquipmentItemType.Armor) shopItemBuyStatusText.text = "체력 + " + player.shopSlots[inventorySlotNum].inventoryItem.health.ToString();
            else if (player.shopSlots[inventorySlotNum].inventoryItem.equipmentItemType == EquipmentItemType.Glove) shopItemBuyStatusText.text = "공격속도 + " + player.shopSlots[inventorySlotNum].inventoryItem.attackSpeed.ToString();
            else if (player.shopSlots[inventorySlotNum].inventoryItem.equipmentItemType == EquipmentItemType.Shoes) shopItemBuyStatusText.text = "이동속도 + " + player.shopSlots[inventorySlotNum].inventoryItem.moveSpeed.ToString();
            else if (player.shopSlots[inventorySlotNum].inventoryItem.equipmentItemType == EquipmentItemType.Amulet) shopItemBuyStatusText.text = "크리티컬확률 + " + player.shopSlots[inventorySlotNum].inventoryItem.criticalPercentage.ToString() + " %" + System.Environment.NewLine + "크리티컬데미지 + " + player.shopSlots[inventorySlotNum].inventoryItem.criticalDamage.ToString() + " %";
            else if (player.shopSlots[inventorySlotNum].inventoryItem.equipmentItemType == EquipmentItemType.Pet) shopItemBuyStatusText.text = "귀여움 + " + player.shopSlots[inventorySlotNum].inventoryItem.cute.ToString();
        }

        // 소모품
        if (player.shopSlots[inventorySlotNum].inventoryItem.itemType == ItemType.Consumables) shopItemBuyStatusText.text = "HP회복 + " + player.shopSlots[inventorySlotNum].inventoryItem.healingPoint.ToString();

        // 인벤토리 슬롯 확장권
        if (player.shopSlots[inventorySlotNum].inventoryItem.itemType == ItemType.ExpansionSlot) shopItemBuyStatusText.text = "인벤토리 슬롯 4칸을 확장합니다";

        // 랜덤 스킬
        if (player.shopSlots[inventorySlotNum].inventoryItem.itemType == ItemType.RandomSkill) shopItemBuyStatusText.text = "영구적으로 적용되는 스킬을 얻습니다";

        // 액티브스킬
        if (player.shopSlots[inventorySlotNum].inventoryItem.itemType == ItemType.Ability) shopItemBuyStatusText.text = player.shopSlots[inventorySlotNum].inventoryItem.skillContent;

        // 랜덤 펫
        if (player.shopSlots[inventorySlotNum].inventoryItem.itemType == ItemType.RandomPet) shopItemBuyStatusText.text = "펫이 들어있다";

        // 상점 아이템 번호를 스태틱변수에 저장
        InventorySlot.inventorySlotNumSave = inventorySlotNum;

        // 사운드
        SoundManager.instance.SFXPlay(ObjType.버튼소리);
    }

    public void QuitShopItemBuyPanel()
    {
        // 상점 아이템 구매 패널 비활성화
        shopItemBuyPanel.SetActive(false);

        // 사운드
        SoundManager.instance.SFXPlay(ObjType.버튼소리);
    }

    public void BuyShopItem()
    {
        // 상점 아이템을 구매하는 함수
        // 플레이어의 코인이 아이템의 가격보다 많으면서 인벤토리 슬롯이 남아있을때
        if(player.coin >= player.shopSlots[InventorySlot.inventorySlotNumSave].inventoryItem.price && Inventory.instance.inventoryItems.Count < Inventory.instance.InventorySlotCnt)
        {
            // 구매
            // 코인 차감
            player.coin -= player.shopSlots[InventorySlot.inventorySlotNumSave].inventoryItem.price;

            // 구매한 아이템 인벤토리에 추가
            Inventory.instance.AddInventoryItem(player.shopSlots[InventorySlot.inventorySlotNumSave].inventoryItem);

            // 상점 아이템 구매 패널 비활성화
            shopItemBuyPanel.SetActive(false);

            // 사운드
            SoundManager.instance.SFXPlay(ObjType.버튼소리);

            // 카운트베이스 퀘스트 처리 -> 얘는 카운팅만하면 자동으로 Check함
            foreach (QuestBase quest in QuestManager.instance.QuestList)
            {
                if (quest is ShopBuyQuest)
                {
                    // 상점구매퀘스트면
                    ShopBuyQuest shopBuyQuest = quest as ShopBuyQuest;
                    
                    // 퀘스트에서 타겟 아이템타입 돌면서
                    for(int i = 0; i < shopBuyQuest.itemType.Count; i++)
                    {
                        // 타겟 아이템 타임을 샀으면
                        if(shopBuyQuest.itemType[i] == player.shopSlots[InventorySlot.inventorySlotNumSave].inventoryItem.itemType)
                        {
                            // 카운트베이스 가져와서 카운팅
                            CountBase countBase = quest as CountBase;
                            countBase.CurCnt++;

                            // 퀘스트 진행상황 알림
                            QuestManager.instance.QuestNotify($"{countBase.questName} {countBase.CurCnt}/{countBase.completeCnt}");

                            return;
                        }
                    }
                }
            }
        }
        else
        {
            // 구매 불가
            // 사운드 : 장비 장착 실패 소리와 같음
            SoundManager.instance.SFXPlay(ObjType.장비장착실패소리);
        }
    }
}
