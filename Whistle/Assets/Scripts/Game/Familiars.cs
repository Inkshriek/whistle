using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whistle.Familiars {

    public abstract class Familiar : MonoBehaviour {
        public abstract string Name { get; set; }
        public abstract FamiliarState State { get; set; }
        public abstract GameObject Player { get; set; }
        public abstract string Speed { get; set; }

        protected virtual void MovePosition() {

        }

        protected abstract void PrimaryAction();

        public abstract void Activate();
        public abstract void Deactivate();
    }

    public enum FamiliarState {
        Ground,
        Water,
        Air
    }

    public enum FamiliarAIType {
        Normal,
        Flying
    }

}
