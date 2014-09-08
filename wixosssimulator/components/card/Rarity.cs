using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wixosssimulator.components.card
{
    /// <summary> カードのレアリティを表す列挙値を提供します。 </summary>
    public enum Rarity
    {
        C, R, LC, SR, LR, Secret = 9,
        PR = 13,
        ST = 23,
        SP = 53,
        Question = 99
    }
}

//Rarity.php

    ///** 不明
    // * @var int */
    //const UNKNOWN = -1;

    ///** コモン (COMMON)<br>●C
    // * @var int */
    //const C = 1;
    ///** レア (RARE)<br>●●R
    // * @var int */
    //const R = 2;
    ///** ルリグコモン (LRIG COMMON)<br>●●●LC
    // * @var int */
    //const LC = 3;
    ///** スーパーレア (SUPER RARE)<br>●●●●SR
    // * @var int */
    //const SR = 4;
    ///** ルリグレア (LRIG RARE)<br>●●●●●LR
    // * @var int */
    //const LR = 5;

    ///** シークレット (SECRET)<br>●SECRET●
    // * @var int */
    //const SECRET = 9;

    ///** <i>[TBC]</i> プロモーションカード (PROMOTION CARD)<br>●●●PR
    // * @var int */
    //const PR = 13;

    ///** <i>[TBC]</i> 構築済みデッキ (STRUCTURE DECK)<br>●●●ST
    // * @var int */
    //const ST = 23;

    ///** <i>[TBC]</i> (SPEC, SPECIAL) <br>●●●SP
    // * @var int */ // ((SPECIFICATION)) [spec selector]
    //const SP = 53;

    ///** ？？？ (???)<br>●???●
    // * @var int */
    //const QUESTION = 99;
