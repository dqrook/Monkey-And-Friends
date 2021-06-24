var localStorage = {

  saveToLocalStorage: function(key, data) {
    localStorage.setItem(Pointer_stringify(key), Pointer_stringify(data));
  },

  loadFromLocalStorage: function(key) {
    var returnStr = localStorage.getItem(Pointer_stringify(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  removeFromLocalStorage: function(key) {
    localStorage.removeItem(Pointer_stringify(key));
  },

  hasKey: function(key) {
    if (localStorage.getItem(Pointer_stringify(key))) {
      return 1;
    }
    else {
      return 0;
    }
  }
};

mergeInto(LibraryManager.library, localStorage);;