import numpy as np
from PIL import Image
from pickle import load
import matplotlib.pyplot as plt
from keras.models import load_model
from keras.preprocessing.image import load_img, img_to_array
from utils.model import CNNModel, generate_caption_beam_search
import os
from config import config, get_absolute_path

"""
    *Some simple checking
"""
assert type(config['max_length']) is int, 'Please provide an integer value for `max_length` parameter in config.py file'
assert type(config['beam_search_k']) is int, 'Please provide an integer value for `beam_search_k` parameter in config.py file'


def extract_features(filename, model, model_type):
    if model_type == 'inceptionv3':
        from keras.applications.inception_v3 import preprocess_input
        target_size = (299, 299)
    elif model_type == 'vgg16':
        from keras.applications.vgg16 import preprocess_input
        target_size = (224, 224)
    else:
        raise ValueError()

    image = load_img(filename, target_size=target_size)
    image = img_to_array(image)
    image = image.reshape((1, image.shape[0], image.shape[1], image.shape[2]))
    image = preprocess_input(image)
    features = model.predict(image, verbose=0)
    return features


class CaptionGenerator:
    def __init__(self):
        tokenizer_path = get_absolute_path(config['tokenizer_path'])
        with open(tokenizer_path, 'rb') as f:
            self._tokenizer = load(f)
        self._max_length = config['max_length']
        self._caption_model = load_model(
            get_absolute_path(config['model_load_path']))
        self._image_model = CNNModel(config['model_type'])

    def get_caption(self, path):
        if os.path.splitext(path)[1] not in ['.jpg', '.gif']:
            raise ValueError()

        image = extract_features(path, self._image_model, config['model_type'])

        generated_caption = generate_caption_beam_search(
            self._caption_model, self._tokenizer, image, self._max_length,
            beam_index=config['beam_search_k'])

        caption = generated_caption.split()[1].capitalize()
        for x in generated_caption.split()[
                 2:len(generated_caption.split()) - 1]:
            caption = caption + ' ' + x

        return caption


def main():
    try:
        generator = CaptionGenerator()
        print('loaded')
    except Exception as e:
        print('error')
        raise e

    data = input()
    while data != "end":
        caption = generator.get_caption(data)
        print(caption)
        data = input()


if __name__ == '__main__':
    main()
