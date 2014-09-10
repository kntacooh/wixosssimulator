using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WixossSimulator.Card
{
    /// <summary> カードのレアリティを表す列挙値を提供します。 </summary>
    public enum RarityKind
    {
        /// <summary> コモン (COMMON) </summary>
        C,
        /// <summary> レア (RARE) </summary>
        R,
        /// <summary> ルリグコモン (LRIG COMMON) </summary>
        LC,
        /// <summary> スーパーレア (SUPER RARE) </summary>
        SR,
        /// <summary> ルリグレア (LRIG RARE) </summary>
        LR,
        /// <summary> シークレット (SECRET) </summary>
        Secret = 9,
        /// <summary> [未確定] プロモーションカード (PROMOTION CARD) </summary>
        PR = 13,
        /// <summary> [未確定] 構築済みデッキ (STRUCTURE DECK) </summary>
        ST = 23,
        /// <summary> [未確定] (SPEC, SPECIAL)　((SPECIFICATION)) [spec selector?] </summary>
        SP = 53,
        /// <summary> ？？？ (???) </summary>
        Question = 99,
    }

    public enum RaritySign
    {

    }
}

//Rarity.php

    //** コモン (COMMON)<br>●C
    //** レア (RARE)<br>●●R
    //** ルリグコモン (LRIG COMMON)<br>●●●LC
    //** スーパーレア (SUPER RARE)<br>●●●●SR
    //** ルリグレア (LRIG RARE)<br>●●●●●LR

    //** シークレット (SECRET)<br>●SECRET●

    //** <i>[TBC]</i> プロモーションカード (PROMOTION CARD)<br>●●●PR

    //** <i>[TBC]</i> 構築済みデッキ (STRUCTURE DECK)<br>●●●ST

    //** <i>[TBC]</i> (SPEC, SPECIAL) <br>●●●SP

    //** ？？？ (???)<br>●???●
