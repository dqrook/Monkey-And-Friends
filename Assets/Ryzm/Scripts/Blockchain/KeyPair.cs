/*
 *This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 */

using System;
using UnityEngine;

namespace Ryzm.Blockchain
{
    public class KeyPair
    {
        public string publicKey;
        public string secretKey;

        public KeyPair(Byte[] publicKey, Byte[] secretKey)
        {
            Debug.Log("secret len" + secretKey.Length);
            this.publicKey = Base58.Encode(publicKey);
            this.secretKey = Base58.Encode(secretKey);
        }
    }
}

