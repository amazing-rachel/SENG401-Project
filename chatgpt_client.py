# import os module to read environment variables
import os
# load_dotenv() reads the .env file and load key/value into environment variables
from dotenv import load_dotenv
# used to create chatgpt client
from openai import OpenAI


def get_gpt_client() -> tuple[OpenAI, str]:
    """
    Returns (client, model_name).
    Loads GPT_API_KEY and GPT_MODEL from .env or environment variables.
    """
    load_dotenv()

    # get the api key from .env
    api_key = os.getenv("GPT_API_KEY")
    # look for chatgpt model, if not found, default to gpt-4.1-mini
    model = os.getenv("GPT_MODEL", "gpt-4.1-mini")

    if not api_key:
        raise RuntimeError(
            "Missing GEMINI_API_KEY. Create a .env file or set it in your environment."
        )

    # create client to call the model
    client = OpenAI(api_key=api_key)
    return client, model